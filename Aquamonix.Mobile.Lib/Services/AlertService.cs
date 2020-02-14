using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Services
{
    /// <summary>
    /// Interface to alerts service.
    /// </summary>
	public interface IAlertService : IService
    {
        /// <summary>
        /// Requests alerts for the given device.
        /// The response from this call may be cached temporarily on the service layer.
        /// </summary>
        /// <param name="device">The device for which to start circuits</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<AlertsResponse> RequestAlerts(Device device, Action onReconnect = null, bool silentMode = false);

        /// <summary>
        /// Requests alerts for all specified devices.
        /// The response from this call may be cached temporarily on the service layer.
        /// </summary>
        /// <param name="deviceIds">The devices for which to get alerts</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<AlertsResponse> RequestGlobalAlerts(IEnumerable<string> deviceIds, Action onReconnect = null, bool silentMode = false);     

        Task<DevicesResponse> DismissDeviceAlerts(string deviceId, IEnumerable<string> alertIds, Action onReconnect = null, bool silentMode = false);

        Task<DevicesResponse> DismissGlobalAlerts(IEnumerable<string> alertIds, Action onReconnect = null, bool silentMode = false); 
    }

    /// <summary>
    /// Implementation of alerts service.
    /// </summary>
	public class AlertService : ServiceBase, IAlertService
	{
        public async Task<DevicesResponse> DismissDeviceAlerts(string deviceId, IEnumerable<string> alertIds, Action onReconnect = null, bool silentMode = false)
        {
            return await (this.DismissAlerts(alertIds, onReconnect, silentMode, global:false, deviceId:deviceId));
        }

        public async Task<DevicesResponse> DismissGlobalAlerts(IEnumerable<string> alertIds, Action onReconnect = null, bool silentMode = false)
        {
            return await (this.DismissAlerts(alertIds, onReconnect, silentMode, global:true));
        }

        public async Task<AlertsResponse> RequestAlerts(Device device, Action onReconnect = null, bool silentMode = false)
		{
			try
			{
                //prepare request
				var deviceRequest = new DeviceRequest(device);
				var request = new AlertsRequest(new DeviceRequest[] { deviceRequest });

                //get response
				var response = await this.DoRequest<AlertsRequest, AlertsResponse>(request, withCaching:false, onReconnect:onReconnect, silentMode:silentMode);
				
                //after-response tasks
                this.UpdateDevices(response, new string[] { device.Id});

				return response;
			}
			catch (Exception e)
			{
				LogUtility.LogException(e);
			}

			return null;
		}

		public async Task<AlertsResponse> RequestGlobalAlerts(IEnumerable<string> deviceIds, Action onReconnect = null, bool silentMode = false)
		{
			try
			{
                //prepare request
				var request = new AlertsRequest(deviceIds);
                
                //get response
				var response = await this.DoRequest<AlertsRequest, AlertsResponse>(request, withCaching: false, onReconnect:onReconnect, silentMode: silentMode);
				
                //after-response tasks
                this.UpdateDevices(response, deviceIds);

				if (response != null && response.IsSuccessful)
				{
					DataCache.SetActiveAlertsCount(response.Body?.ActiveAlertsCount);
				}

				return response;
			}
			catch (Exception e)
			{
				LogUtility.LogException(e);
			}

			return null;
		}


		private void UpdateDevices(AlertsResponse alertsResponse, IEnumerable<string> deviceIds)
		{
			Dictionary<string, Device> devices = new Dictionary<string, Device>();
			foreach (var id in deviceIds)
				devices.Add(id, new Device() { Id = id, Alerts = new ItemsDictionary<Alert>() }); 

			if (alertsResponse != null && alertsResponse.IsSuccessful && alertsResponse.Body.Alerts != null)
			{
				foreach (var alert in alertsResponse.Body.Alerts.Values)
				{
					string deviceId = alert.DeviceId;
					if (deviceId != null && devices.ContainsKey(deviceId))
						devices[deviceId].Alerts.Add(alert.Id, alert);
				}

				foreach (var device in devices.Values)
					DataCache.AddOrUpdate(device);
			}
		}

        private async Task<DevicesResponse> DismissAlerts(IEnumerable<string> alertIds, Action onReconnect = null, bool silentMode = false, bool global = false, string deviceId = null)
        {
            try
            {
                //prepare request
                var request = new DismissAlertRequest(alertIds);

                //get response
                var response = await this.DoRequest<DismissAlertRequest, DevicesResponse>(request, withCaching: false, onReconnect: onReconnect, silentMode: silentMode);

                //after-response tasks
                if (response != null && response.Body != null)
                {
                    if (global)
                    {
                        if (response.Body.ActiveAlertsCount != null)
                        {
                            DataCache.SetActiveAlertsCount(response.Body.ActiveAlertsCount);
                        }

                        if (response.Body.Alerts != null)
                        {
                            DataCache.CacheAlerts(response.Body.Alerts.Values, deleteNonexisting: true);
                        }
                    }
                    else
                    {
                        if (response.Body.Alerts != null)
                        {
                            DataCache.CacheAlertsForDevice(deviceId, response.Body.Alerts.Values, deleteNonexisting: true);
                        }
                    }
                }

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
