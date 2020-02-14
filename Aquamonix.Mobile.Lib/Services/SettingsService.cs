using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.Lib.Services
{
    /// <summary>
    /// Service interface for settings-related calls.
    /// </summary>
	public interface ISettingsService : IService
	{
        /// <summary>
        /// Sets settings, e.g. program scale output. 
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to set the setting</param>
        /// <param name="setting">The setting to set or change, contains the new value</param>
        /// <param name="settingId">ID of the setting to change</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> SetSettings(Device device, DeviceSetting setting, string settingId, Action<ProgressResponse> handleUpdates, Action onReconnect = null, bool silentMode = false);
	}

    /// <summary>
    /// Implementation of settings service.
    /// </summary>
	public class SettingsService : ServiceBase, ISettingsService
	{
		public async Task<ProgressResponse> SetSettings(Device device, DeviceSetting setting, string settingId, Action<ProgressResponse> handleUpdates, Action onReconnect = null, bool silentMode = false)
		{
			try
			{
                //create a copy of the device to send in the request
				var deviceCopy = new Device() { Id = device.Id }; 
				if (device.MetaData != null)
				{
					deviceCopy.MetaData = new DeviceMetadata() { TimeStamp = device.MetaData.TimeStamp }; 
				}

                //create & configure request
				var deviceRequest = new DeviceRequest(deviceCopy);

				var settingClone = setting.Clone();
				deviceRequest.AddSetting(settingClone);
				settingClone.Id = null;

                //send request
				var request = new SetStatusesRequest(deviceRequest);
				var response = await this.DoRequest<SetStatusesRequest, ProgressResponse>(request, onReconnect:onReconnect, silentMode: silentMode);

                //clear cache & other maintenance
				ServiceContainer.InvalidateCache();
				this.RegisterForProgressUpdates(response, handleUpdates);

				return response;
			}
			catch (Exception e)
			{
				LogUtility.LogException(e);
			}

			return null;
		}
	}
}
