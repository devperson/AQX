using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UIKit;
using Foundation;

using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.Utilities.WebSockets
{
    /// <summary>
    /// Handles websocket events related to the connection state; manages the process of reconnecting both in background and foreground.
    /// </summary>
    public static class ConnectionManager
    {
        //how long to wait between retries, for server-down
        private const int ServerDownRetryWaitSeconds = 30;

        //how long to wait between retries, for auth retries (no longer relevant)
        private const int AuthRetryWaitSeconds = 30; 

        //default wait in between retries
        private const int DefaultRetryWaitSeconds = 10;

        //how long it takes for a disconnected connection to be considered 'dead' (timed from beginning of reconnect process)
        private const int ConnectionDeathSeconds = 100;

        //how long the connection must be down before we can display the reconnecting banner (timed from beginning of reconnect process)
        private const int BannerDelaySeconds = 5; 

        private static ConnectionState _state = ConnectionState.Disconnected;
        private static ConnectionEventHandler _onConnected = null;

        //true if we have not yet successfully connected to server in lifetime of app
        public static bool InitialConnection = false;

        /// <summary>
        /// Gets the current state of connection 
        /// </summary>
        public static ConnectionState State { get { return _state; } }

        /// <summary>
        /// Returns true if currently reconnecting, either in foreground or background 
        /// </summary>
        public static bool IsReconnecting { get { return ReconnectProcess.Running; } }

        /// <summary>
        /// Returns true if currently showing reconnecting banner 
        /// </summary>
        public static bool ShowingReconBar { get { return ReconnectProcess.IsShowingReconBar; } }


        /// <summary>
        /// Static constructor
        /// </summary>
        static ConnectionManager()
        {
            SetState(ConnectionState.Disconnected); 
        }


        /// <summary>
        /// Desubscribes all websocket events 
        /// </summary>
        public static void Deinitialize()
        {
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.ConnectionOpened -= OnConnectionOpened;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.ConnectionClosed -= OnConnectionClosed;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.RequestSucceeded -= OnRequestSucceeded;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.ConnectionFailed -= OnConnectionFailed;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.RequestFailed -= OnRequestFailed;
        }

        /// <summary>
        /// Initializes instance; subscribes all websocket events 
        /// </summary>
        public static void Initialize()
        {
            //unsubscribe first
            Deinitialize();

            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.ConnectionOpened += OnConnectionOpened;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.ConnectionClosed += OnConnectionClosed;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.RequestSucceeded += OnRequestSucceeded;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.ConnectionFailed += OnConnectionFailed;
            Aquamonix.Mobile.Lib.Utilities.WebSockets.WebSocketsClient.RequestFailed += OnRequestFailed;
        }

        /// <summary>
        /// Attempts to cancel any reconnection currently happening
        /// </summary>
        /// <returns>True on successful cancel (async)</returns>
        public static async Task<bool> CancelReconnecting()
        {
            if (ReconnectProcess.Running)
            {
                await ReconnectProcess.Cancel();

                if (WebSocketsClient.IsConnected)
                    SetState(ConnectionState.Connected);
                else
                    SetState(ConnectionState.Disconnected, "cancelled"); 
            }

            return true;
        }


        //EVENT HANDLERS 

        private static void OnConnectionOpened(object sender, ConnectionEventArgs e)
        {
            LogUtility.LogMessage(String.Format("Connection opened"));
            SetState(ConnectionState.Connected);

            if (_onConnected != null)
                _onConnected(sender, e); 
        }

        private static void OnConnectionFailed(object sender, ConnectionEventArgs e)
        {
            LogUtility.LogMessage(String.Format("Connection failed"));
            SetState(ConnectionState.Disconnected);
        }

        private static void OnConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            LogUtility.LogMessage("Connection closed");
            SetState(ConnectionState.Disconnected);

            if (ReconnectIsAllowed())
            {
                ReconnectProcess.Begin();
            }
        }

        private static void OnRequestSucceeded(object sender, RequestEventArgs e)
        {
            LogUtility.LogMessage(String.Format("Request succeeded"));
            //InitialConnection = true; 
            SetState(ConnectionState.Connected);
        }

        private static void OnRequestFailed(object sender, RequestFailureEventArgs e)
        {
            HandleFailedRequest(sender as IWebSocketsClient, e); 
        }

        /// <summary>
        /// Handles a failed websocket request 
        /// </summary>
        /// <param name="client">The websocket client which sent the request</param>
        /// <param name="args">Arguments associated with failure</param>
        private static void HandleFailedRequest(IWebSocketsClient client, RequestFailureEventArgs args)
        {
            LogUtility.LogMessage(String.Format("Request Failure (reason: {0})", Enum.GetName(typeof(RequestFailureReason), args.FailureReason)));

            bool tryReconnect = false;
            string message = null;

            switch (args.FailureReason)
            {
                case RequestFailureReason.Auth:
                    tryReconnect = false;
                    message = "authentication problem";
                    NotificationUtility.PostNotification(NotificationType.AuthFailure);
                    break;

                case RequestFailureReason.Timeout:
                    tryReconnect = ReconnectIsAllowed();
                    message = "timeout";
                    break;

                case RequestFailureReason.ServerDown:
                    tryReconnect = ReconnectIsAllowed();
                    message = "server down";
                    break;

                case RequestFailureReason.Network:
                    tryReconnect = ReconnectIsAllowed();
                    message = "network issue";
                    break;

                case RequestFailureReason.ServerRequestedReconnect:
                    tryReconnect = ReconnectIsAllowed();
                    message = "server requested reconnect";
                    break;

                case RequestFailureReason.Error:
                    break;
            }

            if (tryReconnect)
            {
                SetState(ConnectionState.Disconnected, message);
                ReconnectProcess.Begin(args.ForegroundAction, args.OnResume, args.Request, args.FailureReason);
            }
        }

        /// <summary>
        /// Internally sets the instance connection state
        /// </summary>
        /// <param name="state">The state value to set</param>
        /// <param name="message">Optional message to display on banner</param>
        private static void SetState(ConnectionState state, string message = null)
        {
            state.Message = message;
            if (_state != state)
            {
                LogUtility.LogMessage(String.Format("Connection state set to {0}", state.ToString()));
                _state = state;
                NotificationUtility.PostNotification(NotificationType.ConnectionStateChanged);
            }
        }

        /// <summary>
        /// Under certain circumstances, reconnect may not be allowed (e.g. on login screen) 
        /// </summary>
        private static bool ReconnectIsAllowed()
        {
            return !(ViewControllers.TopLevelViewControllerBase.CurrentViewController is ViewControllers.UserConfigViewController);
        }


        /// <summary>
        /// Handles the recurrent process of actually attempting to restore a dropped connection.
        /// </summary>
        public static class ReconnectProcess
        {
            //keeps count of the number of attempts (first attempt is 1)
            private static int _attemptNumber = 0;

            //optional limit on number of attempts allowed (default is 0 == infinite)
            private static int _limitAttempts = 0; 

            //unix timestamp indicating the time of most recent reconnection process start 
            private static long _startTimestamp = DateTimeUtil.GetUnixTimestamp();

            //used in cancellation 
            private static AutoResetEvent _cancelHandle = null;

            //true if currently showing a progress spinner as part of reconnection process
            private static bool _showingProgress = false;

            //true if currently showing reconnection banner 
            private static bool _showingReconBar = false;

            //1 when running, 0 when not 
            private static int _runFlag = 0;

            //true if running in background (as opposed to foreground) 
            private static bool _runningInBackground = false;

            //reason for reconnection (original)
            private static RequestFailureReason? _originalReason = null;

            //reason for reconnection (most recently assigned, may be different from original)
            private static RequestFailureReason? _latestReason = null;

            //connection dead 
            private static bool _connectionDead = false;

            /// <summary>
            /// Returns true if currently running in foreground or background 
            /// </summary>
            public static bool Running
            {
                get { return _runFlag == 1; }
            }

            /// <summary>
            /// Returns true if the connection's been down and unrecoverable for a long enough time.
            /// </summary>
            public static bool Dead { get { return _connectionDead; } }

            /// <summary>
            /// Returns true if currently running in background 
            /// </summary>
            public static bool RunningInBackground { get { return _runningInBackground; } }

            /// <summary>
            /// Returns true if currently showing reconnecting banner 
            /// </summary>
            public static bool IsShowingReconBar { get { return _showingReconBar; } }

            /// <summary>
            /// Returns true if reconnecting for an auth failure
            /// </summary>
            private static bool IsAuthFailure { get { return (_latestReason != null && _latestReason == RequestFailureReason.Auth); } }

            /// <summary>
            /// Starts the whole process running 
            /// </summary>
            /// <param name="foregroundAction">True if we should try first in foreground before moving to background</param>
            /// <param name="onResume">Optional action to retry if connection restored in foreground</param>
            /// <param name="request">The request associated with retry attempt (e.g. the failed request)</param>
            /// <param name="reason">The reason for the need to reconnect</param>
            public static void Begin(bool foregroundAction = false, Action onResume = null, IApiRequest request = null, RequestFailureReason? reason = null, bool showBannerStraightAway = false)
            {
                ExceptionUtility.Try(() => {
                    if (reason != null)
                    {
                        if (_originalReason == null && reason != null)
                            _originalReason = reason;

                        _latestReason = reason;
                    }

                    if (showBannerStraightAway) {
                        ShowReconBanner();
                    }

                    Task.Factory.StartNew(() => { BeginInternal(foregroundAction, onResume, request); });
                });
            }

            /// <summary>
            /// Attempts to cancel the reconnection proces
            /// </summary>
            /// <returns>true on successful cancel</returns>
            public static Task<bool> Cancel()
            {
                return Task.Run<bool>(() => {
                    return ExceptionUtility.Try<bool>(() =>
                    {
                        _attemptNumber = 0;
                        _limitAttempts = 0;

                        if (ReconnectProcess.Running)
                        {
                            _cancelHandle = new AutoResetEvent(false);

                            //wait for cancel to be recognized
                            bool timedOut = !_cancelHandle.WaitOne(120000);

                            if (_cancelHandle != null)
                            {
                                _cancelHandle.Dispose();
                                _cancelHandle = null;
                            }

                            //returns false if timed out 
                            return !timedOut;
                        }

                        Stop();
                        return true;
                    });
                });
            }

            /// <summary>
            /// Gets the number of seconds since the process began
            /// </summary>
            /// <returns>The number of seconds since starting</returns>
            public static long SecondsSinceStart()
            {
                return (DateTimeUtil.SecondsSinceTimestamp(_startTimestamp));
            }

            /// <summary>
            /// Sets the connection to dead if conditions are met
            /// </summary>
            /// <returns>true if conditions are met to set connection to dead</returns>
            private static bool ConditionalSetConnectionDead()
            {
                //after a while, the connection is considered 'dead' 
                if (SecondsSinceStart() > ConnectionDeathSeconds)
                {
                    LogUtility.LogMessage("Connection is DEAD");
                    ConnectionManager.SetState(ConnectionState.Dead);
                    ShowReconBanner();
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Shows the reconnecting banner if conditions are met to do so.
            /// </summary>
            private static void ConditionalShowReconBanner()
            {
                if (SecondsSinceStart() > BannerDelaySeconds)
                {
                    if (!ShowingReconBar)
                        ProgressUtility.Dismiss();
                    ShowReconBanner();
                }
            }

            /// <summary>
            /// Attempts to reconnect in the foreground while showing a progress spinner
            /// </summary>
            /// <returns>True on successful reconnect</returns>
            private static async Task<bool> ReconnectForeground()
            {
                bool success = false;

                try
                {
                    ShowProgress();
                    LogUtility.LogMessage("Attempting to reconnect in foreground");
                    success = await TryReconnect();

                    if (success)
                        HideProgress();
                }
                catch (Exception e)
                {
                    LogUtility.LogException(e);
                    HideProgress(); 
                    Stop();
                }

                return success;
            }

            /// <summary>
            /// Stops the process (different from cancel, which is async - this is called only internally)
            /// </summary>
            private static void Stop()
            {
                HideReconBanner();
                _runningInBackground = false;
                _runFlag = 0;
                _attemptNumber = 0;
                _limitAttempts = 0;
                _connectionDead = false;
            }

            /// <summary>
            /// Checks for a cancellation attempt, makes the necessary changes if detected.
            /// </summary>
            /// <returns>true if cancelled (big if true)</returns>
            private static bool CheckForCancellation()
            {
                if (_cancelHandle != null)
                {
                    LogUtility.LogMessage("Reconnection process cancelled");
                    _cancelHandle.Set();
                    Stop();
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Attempt to reconnect to server one time
            /// </summary>
            /// <returns>True on successful reconnect</returns>
            private static async Task<bool> TryReconnect()
            {
                try
                {
                    _attemptNumber++; 
                    LogUtility.LogMessage(String.Format("Reconnect attempt {0}", _attemptNumber));
                    ConnectionManager.SetState(ConnectionManager.State.IsDead ? ConnectionState.Dead : ConnectionState.Connecting, String.Format("attempt {0}", _attemptNumber));

                    var response = await ServiceContainer.UserService.RequestConnection(
                                User.Current.Username,
                                User.Current.Password,
                                silentMode: true);


                    bool output = false;
                    if (response != null)
                    {
                        output = response.IsSuccessful;
                    }

                    return output;
                }
                catch (Exception e)
                {
                    LogUtility.LogException(e);
                }

                return false;
            }

            /// <summary>
            /// Display the reconnecting banner 
            /// </summary>
            private static void ShowReconBanner()
            {
                _showingReconBar = true;
                NotificationUtility.PostNotification(NotificationType.ShowReconnecting);
            }

            /// <summary>
            /// Hide the reconnecting banner 
            /// </summary>
            private static void HideReconBanner()
            {
                _showingReconBar = false;
                NotificationUtility.PostNotification(NotificationType.HideReconnecting);
            }

            /// <summary>
            /// Display a progress spinner 
            /// </summary>
            private static void ShowProgress()
            {
                if (!_showingProgress)
                {
                    MainThreadUtility.InvokeOnMain(() => {
                        ProgressUtility.Show("Reconnecting...");
                    });
                    _showingProgress = true;
                }
            }

            /// <summary>
            /// Hide the progress spinner 
            /// </summary>
            private static void HideProgress()
            {
                if (_showingProgress)
                {
                    MainThreadUtility.InvokeOnMain(() => {
                        ProgressUtility.Dismiss();
                    });
                    _showingProgress = false;
                }
            }

            /// <summary>
            /// Takes appropriate actions up on successful reconnect 
            /// </summary>
            /// <param name="onResume">Option action to retry upon successful reconnect</param>
            private static void OnSuccess(Action onResume = null)
            {
                LogUtility.LogMessage("Reconnection success");

                ConnectionManager.SetState(ConnectionState.Connected);
                //NotificationUtility.PostNotification(NotificationType.Reconnected);
                Stop();

                if (onResume != null)
                    onResume();
            }

            /// <summary>
            /// Takes appropriate actions up on failed reconnect 
            /// </summary>
            private static void OnFailure()
            {
                Stop();

                //if auth failure timeout, redirect to login screen 
                if (IsAuthFailure)
                {
                    NotificationUtility.PostNotification(NotificationType.AuthFailure);
                }
            }

            /// <summary>
            /// Waits for n seconds.
            /// </summary>
            private static void WaitSeconds(int sec)
            {
                Thread.Sleep(sec * 1000);
            }

            /// <summary>
            /// Starts the whole process running (to be called internally) 
            /// </summary>
            /// <param name="foregroundAction">True if we should try first in foreground before moving to background</param>
            /// <param name="onResume">Optional action to retry if connection restored in foreground</param>
            /// <param name="request">The request associated with retry attempt (e.g. the failed request)</param>
            private static async void BeginInternal(bool foregroundAction = false, Action onResume = null, IApiRequest request = null)
            {
                try
                {
                    if (Interlocked.CompareExchange(ref _runFlag, 1, 0) == 0)
                    {
                        LogUtility.LogMessage(String.Format("Attempting to reconnect (reason: {0})", (_latestReason == null) ? "null" : Enum.GetName(typeof(RequestFailureReason), _latestReason)));

                        _attemptNumber = 0;
                        _connectionDead = false;
                        _startTimestamp = DateTimeUtil.GetUnixTimestamp();
                        ConnectionManager.SetState(ConnectionState.Connecting);

                        bool reconnectedFirstTime = false;

                        if (CheckForCancellation()) return;

                        //attempt first to reconnect in foreground 
                        if (foregroundAction)
                        {
                            reconnectedFirstTime = await ReconnectForeground();

                            if (reconnectedFirstTime)
                            {
                                LogUtility.LogMessage("Connection restored in foreground");
                                OnSuccess(onResume);
                            }
                            else
                            {
                                //if a prog utility is showing, now we dismiss it cause we're going to background
                                ProgressUtility.Dismiss();

                                ShowReconBanner();

                                //if we're to try once only, then stop here 
                                LogUtility.LogMessage("Connection not restored in foreground; will move to background");
                            }
                        }

                        //first attempt fails 
                        if (!reconnectedFirstTime)
                        {
                            //continue trying in background 
                            while (true)
                            {
                                _runningInBackground = true;

                                //limit the number of attempts for auth failure
                                _limitAttempts = (0);

                                if (CheckForCancellation()) break;

                                //make sure attempt limit isn't exceeded 
                                if (_limitAttempts > 0 && _attemptNumber >= _limitAttempts)
                                {
                                    OnFailure();
                                    break;
                                }

                                //if it's been a while, show the reconnecting banner
                                ConditionalShowReconBanner();
                                ConditionalSetConnectionDead();

                                //the actual reconnect attempt
                                bool reconnected = await TryReconnect();

                                if (CheckForCancellation()) break;

                                //if reconnected, we're good 
                                if (reconnected)
                                {
                                    OnSuccess();
                                    break;
                                }

                                //if it's been a while, show the reconnecting banner
                                ConditionalShowReconBanner();
                                ConditionalSetConnectionDead();

                                //wait in between retries 
                                int waitSec = DefaultRetryWaitSeconds;

                                //for server-down or dead connection, extend the retry wait
                                if ((_latestReason != null && (_latestReason == RequestFailureReason.ServerDown)) || _connectionDead)
                                {
                                    waitSec = ServerDownRetryWaitSeconds;
                                }

                                //wait
                                if (waitSec > 0)
                                {
                                    LogUtility.LogMessage(String.Format("Reconnection process waiting {0} seconds", waitSec));

                                    if (MainThreadUtility.IsOnMainThread)
                                    {
                                        LogUtility.LogMessage("Warning - ConnectionManager should not be waiting on Main Thread!", LogSeverity.Warn);
                                    }

                                    WaitSeconds(waitSec);
                                }
                            }
                        }
                    }
                    else
                    {
                        //could not enter
                        if (foregroundAction)
                        {
                            ProgressUtility.Dismiss();
                            ShowReconBanner(); 
                        }
                    }
                }
                catch (Exception e)
                {
                    LogUtility.LogException(e);
                }
            }
        }
    }
}
