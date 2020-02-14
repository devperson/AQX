using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Services 
{
    /// <summary>
    /// Base class for all service implementations. 
    /// 
    /// Note that there is a level of very temporary caching of responses implemented as an optimization, here in the 
    /// service base class. Responses which are eligible for caching (which includes some but not all read-only requests) 
    /// will be cached in memory for a short time; until they expire or until they are cleared manually (e.g. on logout) 
    /// </summary>
	public class ServiceBase
	{
        private static ServerVersion _lastServerVersion = ServerVersion.Null; 

        /// <summary>
        /// Number of seconds to cache caching-eligible responses on the service layer.
        /// </summary>
		private const int CacheTimeoutSeconds = 30; 

        public static ServerVersion LastServerVersion {  get { return _lastServerVersion; } }

        /// <summary>
        /// Encapsulates cached responses. 
        /// </summary>
		protected ConcurrentDictionary<string, CacheItem> _internalCache = new ConcurrentDictionary<string, CacheItem>(); 


        /// <summary>
        /// Default constructor
        /// </summary>
		public ServiceBase() { }

        /// <summary>
        /// Clears the service-level cache 
        /// </summary>
		public void ClearCache()
		{
			this._internalCache.Clear();
		}

        /// <summary>
        /// Executes a service request, and returns the response.
        /// </summary>
        /// <param name="request">Encapsulates the request to send.</param>
        /// <param name="withCaching">Specifies whether or not to allow service-level caching for this call</param>
        /// <param name="onReconnect">What to do if the connection is broken, then resumed</param>
        /// <param name="silentMode">If true, will not display any alerts or popups</param>
        /// <returns>The server response</returns>
		protected async Task<TResponse> DoRequest<TRequest, TResponse>(TRequest request, bool withCaching = false, Action onReconnect = null, bool silentMode = false) 
			where TRequest : IApiRequest
			where TResponse : IApiResponse, new()
		{
			bool fromCache = false;

			var response = withCaching ? this.GetFromCache(request) : null;
            if (response == null || !(response is TResponse))
            {
                response = await WebSocketsClient.DoRequestAsync<TRequest, TResponse>(request, onReconnect, silentMode: silentMode);
            }
            else
                fromCache = true;

            if (response?.Header?.ServerVersion != null)
            {
                _lastServerVersion = ServerVersion.Parse(response.Header.ServerVersion);
            }

			if (fromCache)
			{
				LogUtility.LogMessage("Response for service request " + request?.Header?.Type + " returned from cache [Channel : " + request?.Header?.Channel + "].");
			}

			if (!fromCache && withCaching)
			{
				LogUtility.LogMessage("Caching response for service request " + request?.Header?.Type + " [Channel : " + request?.Header?.Channel + "].");
				this.CacheResponse(request, response);
			}

			return (TResponse)response;
		}

        /// <summary>
        /// Registers a callback to receive updates when ProgressResponse updates are received for a specific async command.
        /// </summary>
        /// <param name="response">The first ProgressResponse received; we're saying that we want further updates related to 
        /// this response.</param>
        /// <param name="callback">The callback to call when udpates are received</param>
		protected void RegisterForProgressUpdates(ProgressResponse response, Action<ProgressResponse> callback)
		{
			if (response != null && callback != null)
			{
				if (response.IsFinal)
				{
					callback(response);
				}
				else
				{
					if (response.CommandId != null)
					{
						Action<ProgressResponse> superCallback = (p) =>
						{
							ServiceContainer.InvalidateCache(); 
							callback(p);
						};

						WebSocketsClient.RegisterForProgressUpdates(response.CommandId, superCallback);
					}
				}
			}
		}

        /// <summary>
        /// Caches the response locally in the service layer.
        /// </summary>
        /// <param name="request">The request that prompted the response</param>
        /// <param name="response">The response to cache</param>
		protected void CacheResponse(IApiRequest request, IApiResponse response)
		{
			if (Environment.AppSettings.CachingEnabled)
			{
				ExceptionUtility.Try(() =>
				{
					if (response != null && response.IsSuccessful)
					{
						string key = this.GenerateCacheKey(request);
						var newItem = new CacheItem()
						{
							CacheSeconds = CacheTimeoutSeconds,
							Response = response,
							TimeStamp = DateTime.Now
						};

						this._internalCache.AddOrUpdate(key, newItem, (k, v) =>
						{
							return newItem;
						});
					}
				});
			}
		}

        /// <summary>
        /// Attempts to get a response for the given request, from the cache. 
        /// </summary>
        /// <param name="request">The request for which to get a response from cache.</param>
        /// <returns>The cached response if available & not expired; otherwise null</returns>
		protected IApiResponse GetFromCache(IApiRequest request)
		{
			return ExceptionUtility.Try<IApiResponse>(() =>
			{
				CacheItem cacheItem;
				IApiResponse output = null;
				string key = this.GenerateCacheKey(request);

				if (this._internalCache.TryGetValue(key, out cacheItem))
				{
					if (cacheItem != null)
					{
						if (!cacheItem.IsExpired)
							output = cacheItem.Response;
						else
							this._internalCache.TryRemove(key, out cacheItem);
					}
				}

				return output;
			});
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
		private string GenerateCacheKey(IApiRequest request)
		{
			return ExceptionUtility.Try<string>(() =>
			{
				//remove channel before serializing 
				var channel = request?.Header?.Channel;
				if (request.Header != null)
					request.Header.Channel = null;

				//serialize 
				var output = JsonUtility.Serialize(request);

				//then replace channel 
				if (channel != null)
					request.Header.Channel = channel;

				return output; 
			});
		}


		protected class CacheItem
		{
			public DateTime TimeStamp { get; set;}
			public IApiResponse Response { get; set;}
			public int CacheSeconds { get; set;}
			public bool IsExpired
			{
				get
				{
					return DateTime.Now.Subtract(TimeStamp).TotalSeconds > CacheSeconds;
				}
			}
		}
	}
}
