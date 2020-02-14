using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Square.SocketRocket;
using Foundation;

using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;

//criteria for closing connection: 
// - Closed event received thru websocket delegate
// - No pings in the last N seconds 
// - ReadyState is not open 


namespace Aquamonix.Mobile.IOS.Utilities.WebSockets
{
    /// <summary>
    /// IOS-specific implementation of IWebSocketsClient.
    /// </summary>
    public class WebSocketsClientIos : Aquamonix.Mobile.Lib.Utilities.WebSockets.IWebSocketsClient
    {
        private static int InstanceId               = 0;
        private const int DefaultSendTimeoutMs      = 20000;
        private const int DefaultConnectTimeoutMs   = 15000;  
        private const int SendPingIntervalMs        = 20000;

        private readonly static bool AttachCert = false;
        private const string RaincloudCertName = "SSL/AWS-Raincloud-cert";
        private const string DefaultCertName = "SSL/RemoteScada_Server_PrivateKey";

        private const string RaincloudCertType = "cer";
        private const string DefaultCertType = "pfx";

        //list of callbacks, for processes that are waiting for a websocket connection to open
        private readonly ConcurrentDictionary<string, Action> _waitingForOpen = new ConcurrentDictionary<string, Action>();

        //list of callbacks, for processes that are waiting for a websocket read-only call to respond
        private readonly ConcurrentDictionary<string, Action<IApiResponse>> _waitingForResponse = new ConcurrentDictionary<string, Action<IApiResponse>>();

        //list of callbacks, for processes that are waiting for a websocket write call to respond
        private readonly ConcurrentDictionary<string, Action<ProgressResponse>> _waitingForUpdates = new ConcurrentDictionary<string, Action<ProgressResponse>>();

        //underlying websocket connection object
        private WebSocket _client = null;

        //controls regular pings which are sent to keep the connection open
        private System.Timers.Timer _pingTimer = null;

        //server url
        private string _url;

        //used for logging/diagnostics
        private int _instanceId = 0;
        private bool _shouldBeConnected = false;

        //for debugging/testing
        private static bool PongAlwaysOverdue = false;


        //for debugging/testing connection down conditions 
        private event ConnectionEventHandler _connectionOpened;
        private event ConnectionEventHandler _connectionFailed;
        private event ConnectionClosedEventHandler _connectionClosed;
        private event RequestEventHandler _requestSucceeded;
        private event RequestFailureEventHandler _requestFailed;
          
        public event ConnectionEventHandler ConnectionOpened
        {
            add
            {
                this._connectionOpened += value;
            }
            remove
            {
                if (this._connectionOpened != null)
                    this._connectionOpened -= value;
            }
        }
        public event ConnectionEventHandler ConnectionFailed
        {
            add
            {
                this._connectionFailed += value;
            }
            remove
            {
                if (this._connectionFailed != null)
                    this._connectionFailed -= value;
            }
        }
        public event ConnectionClosedEventHandler ConnectionClosed
        {
            add
            {
                this._connectionClosed += value;
            }
            remove
            {
                if (this._connectionClosed != null)
                    this._connectionClosed -= value;
            }
        }
        public event RequestEventHandler RequestSucceeded
        {
            add
            {
                this._requestSucceeded += value;
            }
            remove
            {
                if (this._requestSucceeded != null)
                    this._requestSucceeded -= value;
            }
        }
        public event RequestFailureEventHandler RequestFailed
        {
            add
            {
                this._requestFailed += value;
            }
            remove
            {
                if (this._requestFailed != null)
                    this._requestFailed -= value;
            }
        }

        public void FireConnectionClosedEvent() 
        {
            LogUtility.LogMessage("firing ConnectionClosed event");
            _shouldBeConnected = false;
            if (_connectionClosed != null) 
                this._connectionClosed(this, new ConnectionClosedEventArgs()); 
        }
        public void FireReconnectEvent()
        {
            LogUtility.LogMessage("firing Reconnect event");
            _shouldBeConnected = false;
            if (_connectionClosed != null)
                this._connectionClosed(this, new ConnectionClosedEventArgs());
        }
        public void FireConnectionFailedEvent()
        {
            LogUtility.LogMessage("firing ConnectionFailed event");
            if (this._connectionFailed != null)
                this._connectionFailed(this, new ConnectionEventArgs());
        }
        public void FireConnectionOpenedEvent()
        {
            LogUtility.LogMessage("firing ConnectionOpened event");
            if (this._connectionOpened != null)
                this._connectionOpened(this, new ConnectionEventArgs());
        }
        public void FireRequestSucceededEvent(IApiRequest request, IApiResponse response)
        {
            LogUtility.LogMessage(String.Format("firing RequestSucceededEvent event (request type: {0})", (request != null ? request.GetType().Name : "<null>")));
            if (this._requestSucceeded != null)
                this._requestSucceeded(this, new RequestEventArgs(request, response));
        }
        public void FireRequestFailureEvent(IApiRequest request, IApiResponse response, RequestFailureReason reason, Action onResume = null, bool foregroundAction = false)
        {
            LogUtility.LogMessage(String.Format("firing RequestFailure event (request type:{0}, reason:{1}, callback:{2}, foreground:{3})", 
                (request != null ? request.GetType().Name : "<null>"),
                Enum.GetName(typeof(RequestFailureReason), reason), 
                (onResume == null ? "no" : "yes"), 
                (foregroundAction ? "yes" : "no")));

            if (this._requestFailed != null)
                this._requestFailed(this, new RequestFailureEventArgs(request, response, reason, foregroundAction, onResume)); 
        }
        
        /// <summary>
        /// Gets the connection url
        /// </summary>
        public string Url
        {
            get
            {
                string output = String.Empty;
                if (this._client != null && this._client.Url != null)
                    output = this._client.Url.ToString();

                return output;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether or not the websocket is currently connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (this._client == null)
                    return false;

                return this._client.ReadyState == ReadyState.Open;     
            }
        }

        /// <summary>
        /// Gets/sets a boolean flag that determines whether or not the instance will send regular pings to the server, 
        /// to keep the connection open. 
        /// </summary>
        public bool KeepConnectionOpen { get; set; } 

        /// <summary>
        /// Gets a value indicating whether we are overdue for receiving a pong (i.e. we've sent a ping, but not received one in too long)
        /// </summary>
        /// <value>true if overdue</value>
        private bool IsPongOverdue
        {
            get
            {
                bool output = false;
                if (_client != null && _client.Delegate != null)
                {
                    SocketDelegate sockDel = (SocketDelegate)_client.Delegate;
                    return (this.MySocketDelegate.MissedPongs > 1);
                }
                return output;
            }
        }

        private SocketDelegate MySocketDelegate
        {
            get {
                return (_client != null && _client.Delegate != null) ? (_client.Delegate as SocketDelegate) : null; 
            }
        }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public WebSocketsClientIos()
        {
            this._instanceId = InstanceId++;
            this.KeepConnectionOpen = true;
        }

        /// <summary>
        /// Sets the server url. Attaches a certificate if necessary. 
        /// </summary>
        /// <param name="url">The server url</param>
        public void SetUrl(string url)
        {
            this._url = url;
            if (_client != null && _client.ReadyState != ReadyState.Closed)
                _client.Close();

            //_client = new WebSocket(new NSUrl(url.Trim()));
            var nsUrlRequest = new NSMutableUrlRequest(new NSUrl(url.Trim()));

            if (AttachCert)
            {
                this.AttachCertificate(url, nsUrlRequest);
            }

            //NSObject[] protocols = { new NSString("wss"), new NSString("ws") };
            //_client = new WebSocket(nsUrlRequest, protocols, true);

            _client = new WebSocket(nsUrlRequest);

            _client.Delegate = new SocketDelegate(this);
            LogUtility.LogMessage(String.Format("WebSocket Url set to {0}", url));
        }

        /// <summary>
        /// Disconnects from the server if currently connected.
        /// </summary>
        /// <returns>Nothing of note (Task object)</returns>
        public Task Disconnect()
        {
            LogUtility.LogMessage(String.Format("{0}:WebSocket Client Disconnecting", this._instanceId.ToString()));

            return Task.Run(() =>
            {
                ExceptionUtility.Try(() =>
                {
                    _shouldBeConnected = false;
                    if (this._client != null)
                        this._client.Close();
                });
            });
        }

        /// <summary>
        /// The most important method of this class; makes a server call and then asynchronously waits for the response.
        /// </summary>
        /// <param name="request">The request object</param>
        /// <param name="onConnectionResume">Action to execute if the connection is broken, then resumed, during execution of this call.</param>
        /// <param name="silentMode">If true, will not display any popups or alerts for any reason.</param>
        /// <returns>Task result with the response</returns>
        public async Task<TResponse> DoRequestAsync<TRequest, TResponse>(TRequest request, Action onConnectionResume, bool silentMode = false)
        where TRequest : IApiRequest
        where TResponse : IApiResponse, new()
        {
            TResponse response = default(TResponse);

            try
            {
                bool cancelRequest = false;
                //is connection closed? 
                if (_client.ReadyState == ReadyState.Closed || _client.ReadyState == ReadyState.Closing)
                {
                    if (!(request is Aquamonix.Mobile.Lib.Domain.Requests.ConnectionRequest))
                    {
                        this.FireRequestFailureEvent(request, null, RequestFailureReason.Network, onConnectionResume, (!silentMode));
                        cancelRequest = true;
                    }
                    else
                    {
                        _client.Dispose();
                        this.SetUrl(_url);
                    }
                }


                if (!cancelRequest)
                {
                    //attempt to open connection if not already
                    if (_client.ReadyState != ReadyState.Open)
                    {
                        bool opened = await this.Open(request.Header.Channel, silentMode);
                        if (!opened)
                        {
                            //if not a connection request, we want to fire an event  
                            if (!(request is Aquamonix.Mobile.Lib.Domain.Requests.ConnectionRequest))
                            {
                                //fire the event 
                                this.FireRequestFailureEvent(request, null, RequestFailureReason.Network, onConnectionResume, !(silentMode));
                            }

                            return ResponseFactory.FabricateConnectionTimeoutResponse<TResponse>(request);
                        }
                        //NEW: this 'else' clause is new 
                        else
                        {
                            //if not a connection request, then we want to make a connection request now
                            if (!(request is Aquamonix.Mobile.Lib.Domain.Requests.ConnectionRequest))
                            {
                                // make a request connection, if it fails it doesnt matter as it will be picked up below
                                await ServiceContainer.UserService.RequestConnection(
                                    User.Current.Username,
                                    User.Current.Password,
                                    silentMode: true
                                    );
                            }
                        }
                    }

                    //send the request and wait for response
                    response = await this.Send<TResponse>(request, request.Header.Channel, silentMode);

                    //if response is null or unsuccessful, handle 
                    if (response == null || !response.IsSuccessful)
                    {
                        if (response != null && response.IsReconnectResponse)
                        {
                            LogUtility.LogMessage("Received a Reconnect response from server; will reconnect"); 
                            this.FireRequestFailureEvent(request, response, RequestFailureReason.ServerRequestedReconnect, onConnectionResume, (!silentMode));
                        }
                        else
                        {
                            //if response is a timeout, handle 
                            if (response != null && response.IsTimeout)
                            {
                                this.FireRequestFailureEvent(request, response, RequestFailureReason.Timeout, onConnectionResume, (!silentMode));
                            }
                            else
                            {
                                if (response != null)
                                {
                                    //if server is down, handle 
                                    if (response.IsServerDownResponse)
                                    {
                                        this.FireRequestFailureEvent(request, response, RequestFailureReason.ServerDown, onConnectionResume, (!silentMode));
                                    }
                                    else if (response.IsAuthFailure)
                                    {
                                        this.FireRequestFailureEvent(request, response, RequestFailureReason.Auth, onConnectionResume, (!silentMode));
                                    }
                                    else
                                    {
                                        this.FireRequestFailureEvent(request, response, RequestFailureReason.Error, onConnectionResume, (!silentMode));

                                        //show error 
                                        AlertUtility.ShowAppError(response?.ErrorBody);
                                    }
                                }
                                else
                                {
                                    this.FireRequestSucceededEvent(request, response);
                                }
                            }
                        }
                    }
                    else
                    {
                        this.FireRequestSucceededEvent(request, response);
                    }
                }
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
            }

            return response;
        }

        /// <summary>
        /// Registers a callback to receive ProgressResponse updates for an async write call.
        /// </summary>
        /// <param name="commandId">Identifies the command for which we want to receive updates</param>
        /// <param name="callback">Callback to call when updates are received</param>
        public void RegisterForProgressUpdates(string commandId, Action<ProgressResponse> callback)
        {
            ExceptionUtility.Try(() =>
            {
                this.UnregisterWaitingForResponse(commandId);
                _waitingForUpdates.AddOrUpdate(commandId, callback, (arg1, arg2) => callback);
            });
        }

        /// <summary>
        /// Unregisters a previously registered callback from receiving ProgressResponse updates for an async write call.
        /// </summary>
        /// <param name="commandId">Identifies the command for which we want to no longer receive updates</param>
        public void UnregisterFromProgressUpdates(string commandId)
        {
            ExceptionUtility.Try(() =>
            {
                Action<ProgressResponse> action;

                _waitingForUpdates.TryRemove(commandId, out action);
            });
        }

        /// <summary>
        /// Standard Dispose method
        /// </summary>
        public void Dispose()
        {
            _shouldBeConnected = false;
            ExceptionUtility.Try(() =>
            {
                LogUtility.LogMessage(String.Format("{0}:WebSocket Client Disposing", this._instanceId.ToString()));

                this.StopPings();

                if (_client != null)
                {
                    _client.Dispose();

                    if (_client.Delegate != null)
                    {
                        ((SocketDelegate)_client.Delegate).DisposeEx();
                        _client.Delegate = null;
                    }

                    _client = null;
                    _waitingForOpen.Clear();
                    _waitingForResponse.Clear();
                    _waitingForUpdates.Clear();
                }

                //this.UnsubscribeAllEvents();
            });
        }

        /// <summary>
        /// Resets the connection. 
        /// </summary>
        public void RenewConnection()
        {
            this.Dispose();
            this.SetUrl(this._url);
        }

        /// <summary>
        /// Does the work of sending a call to the server, and waiting for the response.
        /// </summary>
        /// <param name="jsonRequest">The request to send</param>
        /// <returns>A Task result with the response</returns>
        private Task<TResponse> Send<TResponse>(IApiRequest jsonRequest, string channel, bool silentMode = false) where TResponse : IApiResponse, new()
        {
            TResponse output = default(TResponse);

            return Task.Run(() =>
            {
                ExceptionUtility.Try(() =>
                {
                    string jsonRequestString = JsonUtility.Serialize(jsonRequest);
                    string jsonRequestStringForLogging = jsonRequestString.Replace(User.Current.Password, "********");
                    LogUtility.LogMessage(String.Format("{2}: Sending Request to {0}: {1}", this.Url, FormatJsonForLogging(jsonRequestStringForLogging), this._instanceId.ToString()));
                    var waitHandle = new AutoResetEvent(false);
                    bool gotResponse = false;

                    Action<IApiResponse> receivedResponse = (r) =>
                    {
                        gotResponse = true;

                        if (r is Lib.Domain.Responses.ConnectionResponse && ((ConnectionResponse)r).IsSuccessful)
                        {
                            NotificationUtility.PostNotification(NotificationType.Reconnected); 
                        }

                        if (r is Lib.Domain.Responses.ErrorResponse)
                        {
                            output = ResponseFactory.ParseResponse<TResponse>(r.RawResponse);
                        }
                        else
                        {
                            try
                            {
                                output = (TResponse)r;
                            }
                            catch(System.InvalidCastException ice)
                            {
                                LogUtility.LogException(ice, String.Format("The server returned a type of response that was not expected. The type of response expected was {0}", (typeof(TResponse)).Name), LogSeverity.Warn);
                                output = default(TResponse); 
                            }
                        }

                        if (r.IsAuthFailure)
                            output = ResponseFactory.FabricateLoginFailureResponse<TResponse>(jsonRequest);

                        gotResponse = true;
                        waitHandle.Set();
                    };

                    RegisterWaitingForResponse(channel, receivedResponse);

                    if (_client != null)
                    {
                        _client.Send(new NSString(jsonRequestString));

                        waitHandle.WaitOne(DefaultSendTimeoutMs);
                        UnregisterWaitingForResponse(channel);
                    }

                    //here we will fabricate an error response for case of timeout 
                    if (output == null && !gotResponse)
                    {
                        output = ResponseFactory.FabricateRequestTimeoutResponse<TResponse>(jsonRequest);
                        this.FireRequestFailureEvent(jsonRequest, null, RequestFailureReason.Timeout, foregroundAction:(!silentMode));
                    }
                });

                return output;
            });
        }

        /// <summary>
        /// Attempts to open a connection to the websocket server.
        /// </summary>
        /// <returns></returns>
        private Task<bool> Open(string channel, bool silentMode = false)
        {
            return Task.Run(() =>
            {
                bool output = false;

                try
                {
                    LogUtility.LogMessage(String.Format("{1}: WebSocket Client opening {0}", this.Url, this._instanceId.ToString()));

                    AutoResetEvent waitHandle = new AutoResetEvent(false);
                    bool gotResponse = false;

                    Action openedResponse = () =>
                    {
                        output = true;
                        gotResponse = true;
                        waitHandle.Set();

                        this._connectionOpened(this, new ConnectionEventArgs()); 
                    };

                    RegisterWaitingForOpen(channel, openedResponse);

                    if (_client.ReadyState != ReadyState.Open)
                    {
                        ExceptionUtility.Try(() =>
                        {
                            _client.Open();
                        });
                    }

                    waitHandle.WaitOne(DefaultConnectTimeoutMs);
                    if (!gotResponse)
                    {
                        LogUtility.LogMessage(String.Format("{0}:Connection timed out.", this._instanceId.ToString()));
                        this.FireRequestFailureEvent(null, null, RequestFailureReason.Network, foregroundAction:(!silentMode)); 
                    }
                    else
                    {
                        _shouldBeConnected = true;
                        if (this.KeepConnectionOpen)
                            this.StartPings();
                    }

                    UnregisterWaitingForOpen(channel);
                }
                catch (Exception e)
                {
                    LogUtility.LogException(e);
                }

                return output;
            });
        }

        /// <summary>
        /// Registers a callback to be called when a connection is open, for the specified channel.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="callback">The callback to call on opening the connection</param>
        private void RegisterWaitingForOpen(string channelId, Action callback)
        {
            _waitingForOpen.AddOrUpdate(channelId, callback, (arg1, arg2) => callback);
        }

        /// <summary>
        /// Unregisters a previously registered callback to be called when a connection is open, for the specified channel.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        private void UnregisterWaitingForOpen(string channelId)
        {
            Action action;
            _waitingForOpen.TryRemove(channelId, out action);
        }

        /// <summary>
        /// Registers a callback to be called when a response is received for a specified channel.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="callback">The callback to call when a response is received</param>
        private void RegisterWaitingForResponse(string channelId, Action<IApiResponse> callback)
        {
            _waitingForResponse.AddOrUpdate(channelId, callback, (arg1, arg2) => callback);
        }

        /// <summary>
        /// Unregisters a previously registered callback to be called when a response is received for a specified channel.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        private void UnregisterWaitingForResponse(string channelId) 
        {
            Action<IApiResponse> action;

            _waitingForResponse.TryRemove(channelId, out action);
        }

        /// <summary>
        /// Starts the timer which controls pings sent to server periodically (to keep the connection open)
        /// </summary>
        private void StartPings()
        {
            ExceptionUtility.Try(() =>
            {
                if (this._pingTimer == null)
                {
                    this._pingTimer = new System.Timers.Timer();

                    this._pingTimer.Enabled = true;
                    this._pingTimer.Interval = SendPingIntervalMs;
                    this._pingTimer.Elapsed += (o, e) =>
                    {
                        ExceptionUtility.Try(() =>
                        {
                            if (this.CheckConnection())
                            {
                                LogUtility.LogMessage(String.Format("{0}:Sending ping", this._instanceId.ToString()));
                                if (this._client != null && this._client.ReadyState == ReadyState.Open)
                                {
                                    ((SocketDelegate)_client.Delegate).IncrementPings();
                                    this._client.SendPing(NSData.FromString("X"));
                                }
                            }
                        });
                    };

                    this._pingTimer.Start();
                }
            });
        }

        /// <summary>
        /// Stops the timer which controls pings sent to server periodically (to keep the connection open)
        /// </summary>
        private void StopPings()
        {
            ExceptionUtility.Try(() =>
            {
                if (this._pingTimer != null)
                {
                    this._pingTimer.Stop();
                    this._pingTimer.Dispose();
                    this._pingTimer = null;
                }
            });
        }

        /// <summary>
        /// Attaches a TLS certificate to be used for encryption of connection.
        /// </summary>
        /// <param name="url">The server url</param>
        /// <param name="nsUrlRequest">The url request to encrypt</param>
        private void AttachCertificate(string url, NSMutableUrlRequest nsUrlRequest)
        {
            if (url.ToLower().Contains("raincloud223.aquamonix.com.au"))
            {
                var certPath = NSBundle.MainBundle.PathForResource(RaincloudCertName, RaincloudCertType);
                //var certData = NSData.FromFile(certPath);
                //Security.SecCertificate secCert = 
                //var certDataRef = (CFDataRef)certData;
                var cert = new Security.SecCertificate(new System.Security.Cryptography.X509Certificates.X509Certificate2(System.IO.File.ReadAllBytes(certPath)));
                //var certPtr = SecCertificateCopyData(cert.Handle);
                nsUrlRequest.SetSSLPinnedCertificates(new NSObject[] { ObjCRuntime.Runtime.GetNSObject(cert.Handle) });
            }
            else
            {
                var certPath = NSBundle.MainBundle.PathForResource(DefaultCertName, DefaultCertType);
                var cert = new Security.SecCertificate(System.Security.Cryptography.X509Certificates.X509Certificate2.CreateFromCertFile(certPath));
                nsUrlRequest.SetSSLPinnedCertificates(new NSObject[] { ObjCRuntime.Runtime.GetNSObject(cert.Handle) });
            }
        }

        /// <summary>
        /// Can be used to shorten or otherwise doctor JSON strings (raw responses & requests) before logging them.
        /// </summary>
        /// <param name="json">The raw JSON string to doctor before logging</param>
        /// <returns>A doctored version of the given json string, suitable for logging</returns>
        private static string FormatJsonForLogging(string json)
        {
            return json;
        }

        /// <summary>
        /// Fires the connectionClosed event if something has unexpectedly happened to the websocket connection. 
        /// </summary>
        private bool CheckConnection()
        {
            if (this.IsConnected)
                _shouldBeConnected = true; 

            if (_shouldBeConnected)
            {
                if (this.IsPongOverdue || !this.IsConnected)
                {
                      _shouldBeConnected = false;
                    this.FireConnectionClosedEvent();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// All listeners to all connection-related events will be unsubscribed. 
        /// </summary>
        private void UnsubscribeAllEvents()
        {
            //unsubscribe all delegates from all events here 
            if (_connectionOpened != null)
            {
                Delegate[] clientList = _connectionOpened.GetInvocationList();
                foreach (var d in clientList)
                {
                    _connectionOpened -= (d as ConnectionEventHandler);
                }
            }
            if (_connectionFailed != null)
            {
                Delegate[] clientList = _connectionFailed.GetInvocationList();
                foreach (var d in clientList)
                {
                    _connectionFailed -= (d as ConnectionEventHandler);
                }
            }
            if (_connectionClosed != null)
            {
                Delegate[] clientList = _connectionClosed.GetInvocationList();
                foreach (var d in clientList)
                {
                    _connectionClosed -= (d as ConnectionClosedEventHandler);
                }
            }
            if (_requestSucceeded != null)
            {
                Delegate[] clientList = _requestSucceeded.GetInvocationList();
                foreach (var d in clientList)
                {
                    _requestSucceeded -= (d as RequestEventHandler);
                }
            }
            if (_requestFailed != null)
            {
                Delegate[] clientList = _requestSucceeded.GetInvocationList();
                foreach (var d in clientList)
                {
                    _requestFailed -= (d as RequestFailureEventHandler);
                }
            }
        }


        /// <summary>
        /// Socket delegate for SocketRocket; does the underlying socket connection work.
        /// </summary>
        private class SocketDelegate : WebSocketDelegate
        {
            private WebSocketsClientIos _client;

            public int MissedPongs { get; private set; }

            private void ResetMissedPongs()
            {
                MissedPongs = 0;
            }

            /// <summary>
            /// Constructor
            /// </summary>  
            /// <param name="client"></param>
            public SocketDelegate(WebSocketsClientIos client)
            {
                if (client == null)
                    throw new NullReferenceException("SocketDelegate's parent client reference cannot be null.");

                _client = client;
            }

            public override void WebSocketOpened(WebSocket webSocket)
            {
                ExceptionUtility.Try(() =>
                {
                    this.ResetMissedPongs();
                    Aquamonix.Mobile.Lib.Utilities.LogUtility.LogMessage(String.Format("{1}:Websocket Open at {0}", this._client.Url, this._client._instanceId.ToString()));
                    _client.FireConnectionOpenedEvent(); 

                    var waitingForOpen = _client._waitingForOpen.ToList();

                    foreach (var item in waitingForOpen)
                    {
                        if (!String.IsNullOrEmpty(item.Key) && item.Value != null)
                        {
                            _client.UnregisterWaitingForOpen(item.Key);
                            item.Value();
                        }
                    }
                });
            }

            public override void WebSocketClosed(WebSocket webSocket, StatusCode code, string reason, bool wasClean)
            {
                // the connection was closed
                ExceptionUtility.Try(() =>
                {
                    this.ResetMissedPongs();
                    LogUtility.LogMessage(String.Format("{0}:Websocket Closed", this._client._instanceId.ToString()));

                    _client.FireConnectionClosedEvent();

                    //reopen 
                    //this._client.RenewConnection();
                });
            }

            public override void WebSocketFailed(WebSocket webSocket, NSError error)
            {
                ExceptionUtility.Try(() =>
                {
                    this.ResetMissedPongs();

                    // there was an error
                    LogUtility.LogMessage(String.Format("{1}:Websocket Failed: {0}", error?.ToString(), this._client._instanceId.ToString()));

                    _client.FireConnectionClosedEvent();
                });
            }

            public override void ReceivedMessage(WebSocket webSocket, NSObject message)
            {
                ExceptionUtility.Try(() =>
                {
                    this.ResetMissedPongs();
                    string msg = (message != null ? message.ToString() : String.Empty);
                    LogUtility.LogMessage(String.Format("{2}:Websocket Message Rcvd (from {0}: {1}", this._client.Url, WebSocketsClientIos.FormatJsonForLogging(msg), this._client._instanceId.ToString()));

                    var waitingForResponse = _client._waitingForResponse.ToList();

                    //deserialize object (or attempt to)
                    var response = ResponseFactory.ParseResponse(msg);

                    //call our callbacks
                    if (response != null && response.Header != null)
                    {
                        foreach (var item in waitingForResponse)
                        {
                            if (item.Value != null)
                            {
                                if ((response.Header.Channel != null && response.Header.Channel == item.Key) || response.IsGlobalResponse)
                                {
                                    _client.UnregisterWaitingForResponse(item.Key);

                                    item.Value(response);
                                }
                            }
                        }
                    }

                    //handle clients waiting for updates 
                    var progressResponse = response as ProgressResponse;
                    if (progressResponse != null)
                    {
                        Action<ProgressResponse> callback;
                        if (_client._waitingForUpdates.TryGetValue(progressResponse.CommandId, out callback))
                        {
                            callback(progressResponse);
                            if (response.IsFinal)
                                _client.UnregisterFromProgressUpdates(progressResponse.CommandId);

                            if (progressResponse?.Body?.Devices != null)
                                DataCache.CacheDevicesResponse((response as ProgressResponse)?.Body?.Devices.Items);
                        }
                    }

                    //handle ad hoc device updates 
                    var devicesResponse = response as DevicesResponse;
                    if (devicesResponse != null)
                    {
                        DataCache.CacheDevicesResponse(devicesResponse);
                    }

                    //handle ad hoc alerts updates 
                    var alertsResponse = response as AlertsResponse;
                    if (alertsResponse != null)
                    {
                        DataCache.HandleAlertsResponse(alertsResponse);
                    }

                    //handle ad hoc device briefs updates 
                    var deviceBriefsResponse = response as DeviceBriefsResponse;
                    if (deviceBriefsResponse != null)
                    {
                        DataCache.CacheDevicesResponse(deviceBriefsResponse?.Body?.Devices.Items);
                    }

                    //handle ad hoc reconnect responses 
                    var errorResponse = response as ErrorResponse;
                    if (errorResponse != null)
                    {
                        if (errorResponse.IsReconnectResponse)
                        {
                            LogUtility.LogMessage("Received an ad hoc Reconnect response from server; will reconnect");
                            _client.FireReconnectEvent();
                        }
                    }
                });
            }

            public override void ReceivedPong(WebSocket webSocket, NSData pongPayload)
            {
                // respond to a ping
                if (!PongAlwaysOverdue)
                    this.ResetMissedPongs();

                LogUtility.LogMessage(String.Format("{0}:Websocket Pong", this._client._instanceId.ToString()), LogSeverity.Debug);
            }

            public void IncrementPings()
            {
                MissedPongs++;
            }

            public void DisposeEx()
            {
                _client = null;
            }
        }
    }
}

