using System;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain.Responses;

namespace Aquamonix.Mobile.Lib.Utilities.WebSockets
{
    /// <summary>
    /// Static helper class for websocket client. 
    /// </summary>
	public static class WebSocketsClient
    {
        public static event ConnectionEventHandler ConnectionOpened 
        {  
            add
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.ConnectionOpened += value;
            } 
            remove 
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.ConnectionOpened -= value; 
            } 
        }
        public static event ConnectionClosedEventHandler ConnectionClosed
        {
            add
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.ConnectionClosed += value;
            }
            remove
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.ConnectionClosed -= value;
            }
        }
        public static event RequestEventHandler RequestSucceeded
        {
            add
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.RequestSucceeded += value;
            }
            remove
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.RequestSucceeded -= value;
            }
        }
        public static event RequestFailureEventHandler RequestFailed
        {
            add
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.RequestFailed += value;
            }
            remove
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.RequestFailed -= value;
            }
        }
        public static event ConnectionEventHandler ConnectionFailed
        {
            add
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.ConnectionFailed += value;
            }
            remove
            {
                if (Environment.Providers.WebSocketsClient != null)
                    Environment.Providers.WebSocketsClient.ConnectionFailed -= value;
            }
        }

        public static bool IsConnected
		{
			get
			{
                if (Environment.Providers.WebSocketsClient != null)
                    return Environment.Providers.WebSocketsClient.IsConnected;

                return false;
			}
		}

		public static void ResetWebSocketsClientUrl(IWebSocketsClient client, string url)
		{
			ExceptionUtility.Try(() =>
			{
				if (Environment.Providers.WebSocketsClient != null)
					Environment.Providers.WebSocketsClient.Dispose();

				client.SetUrl(url);
				Environment.Providers.WebSocketsClient = client;
			});
		}

		public static void RegisterForProgressUpdates(string commandId, Action<ProgressResponse> callback)
		{
			if (Environment.Providers.WebSocketsClient != null)
				Environment.Providers.WebSocketsClient.RegisterForProgressUpdates(commandId, callback);
		}

		public static void UnregisterFromProgressUpdates(string commandId)
		{
			if (Environment.Providers.WebSocketsClient != null)
				Environment.Providers.WebSocketsClient.UnregisterFromProgressUpdates(commandId);
		}

		public static Task<TResponse> DoRequestAsync<TRequest, TResponse>(TRequest request, Action onConnectionResume = null, bool silentMode = false) 
			where TRequest : IApiRequest 
			where TResponse : IApiResponse, new()
		{
			return Environment.Providers.WebSocketsClient.DoRequestAsync<TRequest, TResponse>(request, onConnectionResume, silentMode:silentMode); 
		}

		public async static Task Disconnect()
		{
			if (Environment.Providers.WebSocketsClient != null)
			{
				await Environment.Providers.WebSocketsClient.Disconnect();
				Environment.Providers.WebSocketsClient.Dispose();
			}
		}
	}
}
