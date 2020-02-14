using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Services
{
    /// <summary>
    /// Interface to sensors service.
    /// </summary>
	public interface ISensorService : IService
    {
        /// <summary>
        /// Gets sensors for a given device
        /// </summary>
        /// <param name="device">The device for which to get sensors</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> RequestSensors(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);
	}

    /// <summary>
    /// Implementation of sensors service.
    /// </summary>
	public class SensorService : ServiceBase, ISensorService
	{
		public async Task<ProgressResponse> RequestSensors(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
		{
			try
			{
                //create request/send response
				var request = new SensorsRequest(new DeviceRequest(device));
				var response = await this.DoRequest<SensorsRequest, ProgressResponse>(request, onReconnect: onReconnect, silentMode: silentMode);

                //handle output 
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
