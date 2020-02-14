using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.Lib.Services
{
    /// <summary>
    /// Interface to user service.
    /// </summary>
	public interface IUserService : IService
	{
        /// <summary>
        /// Gets a boolean value indicating whether or not the underlying connection is active.
        /// </summary>
		bool IsConnected { get; }

        /// <summary>
        /// Opens the connection, requests all devices, initializes all caches.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ConnectionResponse> RequestConnection(string username, string password, bool silentMode = false);

        /// <summary>
        /// Logs the current user out. 
        /// This call invalidates the cache and clears the data cache.
        /// </summary>
        /// <returns>Task</returns>
		Task LogOut(); 
	}

    /// <summary>
    /// Implementation of user service.
    /// </summary>
	public class UserService : ServiceBase, IUserService
	{
		public bool IsConnected
		{
			get
			{
				return WebSocketsClient.IsConnected;
			}
		}

		public async Task<ConnectionResponse> RequestConnection(string username, string password, bool silentMode = false)
		{
			try
			{
				string timestamp = null;
				if (DataCache.ApplicationMetadata?.TimeStamp != null)
					timestamp = DataCache.ApplicationMetadata.TimeStamp.ToString();

				//TODO: how to determine the value of the app version
				var request = new ConnectionRequest(username, password, "1", timestamp: timestamp, sessionId: User.Current?.SessionId);

				var output = await WebSocketsClient.DoRequestAsync<ConnectionRequest, ConnectionResponse>(request, silentMode: silentMode);

				ServiceContainer.InvalidateCache();
				if (output != null && output.Body != null)
				{
					if (User.Current != null)
					{
						User.Current.SessionId = output.Body.SessionId;

						if (output.Body.Devices != null)
						{
							if (output.Body.MetaData != null)
							{
								DataCache.ApplicationMetadata = output.Body.MetaData;
							}

							if (output.Body.User?.Name != null)
								User.Current.Name = output.Body.User?.Name;

							if (output.Body.User?.DevicesAccess != null)
								User.Current.DevicesAccess = output.Body.User?.DevicesAccess;

							DataCache.SetDeviceOrder(output.Body?.Devices?.Keys?.ToList());
							DataCache.CacheDevicesResponse(output.Body?.Devices?.Items);
							DataCache.SetActiveAlertsCount(output.Body?.AlertsCount);
							DataCache.SyncCommandProgresses(output.Body?.CommandProgresses); 
						}

						User.Current.Save();
					}
				}

				return output;
			}
			catch (Exception e)
			{
				LogUtility.LogException(e);
			}

			return null;
		}

		public async Task LogOut()
		{
            await WebSocketsClient.Disconnect();
            Caches.ClearCachesForLogout();
		}
	}
}
