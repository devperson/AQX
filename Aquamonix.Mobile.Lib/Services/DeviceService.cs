using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;

namespace Aquamonix.Mobile.Lib.Services
{
    /// <summary>
    /// Interface to devices service.
    /// </summary>
	public interface IDeviceService : IService
	{
		/// <summary>
		/// Requests a device from the server.
        /// The response from this call may be cached temporarily on the service layer.
		/// </summary>
        /// <param name="device">The device to request</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<DevicesResponse> RequestDevice(Device device, Action onReconnect = null, bool silentMode = false);

        /// <summary>
        /// Requests device briefs from the server.
        /// The response from this call may be cached temporarily on the service layer.
        /// </summary>
        /// <param name="device">The list of device ids for which to request briefs</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<DevicesResponse> RequestDeviceBriefs(IEnumerable<string> deviceIds, Action onReconnect = null, bool silentMode = false);

        /// <summary>
        /// Stops irrigations for the device.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to stop irrigations</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> StopIrrigations(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);

        /// <summary>
        /// Calls 'next' irrigations for the device.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to call next</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> NextIrrigations(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);

        /// <summary>
        /// Calls 'prev' irrigations for the device.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to call prev</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> PrevIrrigations(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);
    }

    /// <summary>
    /// Implementation of device service.
    /// </summary>
	public class DeviceService : ServiceBase, IDeviceService
	{
		public async Task<DevicesResponse> RequestDevice(Device device, Action onReconnect = null, bool silentMode=false)
		{
			try
            {
                //send request / get response
				var request = new DevicesRequest(new DeviceRequest(device));
				var response = await this.DoRequest<DevicesRequest, DevicesResponse>(request, withCaching: true, onReconnect:onReconnect, silentMode: silentMode);

				return response;
			}
			catch (Exception e)
			{
				LogUtility.LogException(e);

			}

			return null;
   		}

        public async Task<DevicesResponse> RequestDeviceBriefs(IEnumerable<string> deviceIds, Action onReconnect = null, bool silentMode = false)
		{
            //prepare the request object
			var deviceRequests = new List<DeviceRequest>();
			foreach (var devId in deviceIds)
			{
				var dev = DataCache.GetDeviceFromCache(devId);
				if (dev != null)
					deviceRequests.Add(new DeviceRequest(dev));
				else
					deviceRequests.Add(new DeviceRequest(devId));
			}

            //send request / get response
			var request = new DeviceBriefsRequest(deviceRequests);
			var response = await this.DoRequest<DeviceBriefsRequest, DevicesResponse>(request, withCaching: true, onReconnect:onReconnect, silentMode:silentMode);
            
            //handle after-response tasks
			if (response != null && response.IsSuccessful)
			{
				DataCache.SetDeviceOrder(response.Body?.Devices?.Keys?.ToList());
				DataCache.SetActiveAlertsCount(response.Body?.ActiveAlertsCount); 
			}

			return response;
		}

		public async Task<ProgressResponse> StopIrrigations(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
        {
            //send request / get response
			var request = new StopIrrigationsRequest(new DeviceRequest(device));
			var response = await this.DoRequest<StopIrrigationsRequest, ProgressResponse>(request, onReconnect:onReconnect, silentMode: silentMode);

            //handle after-response tasks
			ServiceContainer.InvalidateCache();
			this.RegisterForProgressUpdates(response, handleUpdates);

			return response;
		}

		public async Task<ProgressResponse> NextIrrigations(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
        {
            //send request / get response
			var request = new NextIrrigationsRequest(new DeviceRequest(device));
			var response = await this.DoRequest<NextIrrigationsRequest, ProgressResponse>(request, onReconnect:onReconnect, silentMode: silentMode);

            //handle after-response tasks
			ServiceContainer.InvalidateCache();
			this.RegisterForProgressUpdates(response, handleUpdates);

			return response;
		}

		public async Task<ProgressResponse> PrevIrrigations(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
        {
            //send request / get response
			var request = new PrevIrrigationsRequest(new DeviceRequest(device));
			var response = await this.DoRequest<PrevIrrigationsRequest, ProgressResponse>(request, onReconnect:onReconnect, silentMode: silentMode);

            //handle after-response tasks
			ServiceContainer.InvalidateCache();
			this.RegisterForProgressUpdates(response, handleUpdates);
			return response;
		}
	}
}
