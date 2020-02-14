using System;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;

namespace Aquamonix.Mobile.Lib.Utilities.WebSockets
{
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);
    public delegate void RequestEventHandler(object sender, RequestEventArgs e);
    public delegate void RequestFailureEventHandler(object sender, RequestFailureEventArgs e);
    public delegate void ConnectionClosedEventHandler(object sender, ConnectionClosedEventArgs e);

    public enum RequestFailureReason
    {
        Network,
        ServerRequestedReconnect,
        Timeout,
        Auth,
        ServerDown,
        Error
    }

    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionEventArgs() : base()
        {

        }
    }

    public class RequestEventArgs: ConnectionEventArgs
    {
        public IApiRequest Request { get; private set; }
        public IApiResponse Response { get; private set; }

        public RequestEventArgs(IApiRequest request, IApiResponse response) : base()
        {
            this.Request = request;
            this.Response = response;
        }
    }

    public class RequestFailureEventArgs : RequestEventArgs
    {
        public RequestFailureReason FailureReason { get; private set; }
        public Action OnResume { get; private set; }
        public bool ForegroundAction { get; private set; }

        public RequestFailureEventArgs(IApiRequest request, IApiResponse response, RequestFailureReason reason, bool foregroundAction) : base(request, response)
        {
            this.FailureReason = reason;
            this.ForegroundAction = foregroundAction;
        }

        public RequestFailureEventArgs(IApiRequest request, IApiResponse response, RequestFailureReason reason, bool foregroundAction, Action onResume) : this(request, response, reason, foregroundAction)
        {
            this.OnResume = onResume;
        }
    }

    public class ConnectionClosedEventArgs : ConnectionEventArgs
    {
        public ConnectionClosedEventArgs() : base()
        {

        }
    }


    /// <summary>
    /// Interface for client-side websockets connection. 
    /// </summary>
	public interface IWebSocketsClient : IDisposable
    {
        event ConnectionEventHandler ConnectionOpened;
        event ConnectionClosedEventHandler ConnectionClosed;
        event ConnectionEventHandler ConnectionFailed;
        event RequestEventHandler RequestSucceeded;
        event RequestFailureEventHandler RequestFailed;

        /// <summary>
        /// Gets a value indicating whether or not the client is currently connected.
        /// </summary>
		bool IsConnected { get; }

        /// <summary>
        /// Sets the server side url.
        /// </summary>
        /// <param name="url">The value to set.</param>
		void SetUrl(string url);

        /// <summary>
        /// Execute a request against the server, and wait for the response.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <param name="callback">Optional callback on completion.</param>
        /// <param name="silentMode">If true, no alerts or popups will be shown</param>
        /// <returns>A Task result</returns>
		Task<TResponse> DoRequestAsync<TRequest, TResponse>(TRequest request, Action callback, bool silentMode = false)
			where TRequest : IApiRequest
			where TResponse : IApiResponse, new();

        /// <summary>
        /// Register a callback for updates from an async write command.
        /// </summary>
        /// <param name="commandId">The id of the command for which to register updates.</param>
        /// <param name="callback">The callback to register.</param>
		void RegisterForProgressUpdates(string commandId, Action<ProgressResponse> callback);

        /// <summary>
        /// Unregister a previously registered callback for updates from an async write command.
        /// </summary>
        /// <param name="commandId">The id of the command from which to unregister updates.</param>
		void UnregisterFromProgressUpdates(string commandId);

        /// <summary>
        /// Disconnects the connection from the server.
        /// </summary>
        /// <returns>A void task.</returns>
		Task Disconnect();
	}
}
