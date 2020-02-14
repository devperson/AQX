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

namespace Aquamonix.Mobile.Lib.Services
{
    /// <summary>
    /// Interface to circuits service.
    /// </summary>
	public interface ICircuitService : IService
    {
        /// <summary>
        /// Starts a given set of circuits.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to start circuits</param>
        /// <param name="circuitIds">List of ids of circuits to start</param>
        /// <param name="durationMinutes">Number of minutes for which to run</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> StartCircuits(Device device, IEnumerable<string> circuitIds, int durationMinutes, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);

        /// <summary>
        /// Stops circuits for a given device.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to stop circuits</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> StopCircuits(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);
	}

    /// <summary>
    /// Implementation of circuits service.
    /// </summary>
	public class CircuitService : ServiceBase, ICircuitService
	{
		public async Task<ProgressResponse> StopCircuits(Device device, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
		{
			try
            {
                //prepare request
				var deviceRequest = new DeviceRequest(device);
				deviceRequest.PassEmptyCircuitsList = true;

				var request = new StartCircuitsRequest(deviceRequest);
                request.Body.DurationMinutes = 0;

                //send request/get response
				var response = await this.DoRequest<StartCircuitsRequest, ProgressResponse>(request, onReconnect:onReconnect, silentMode: silentMode);

                //after-response tasks
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

		public async Task<ProgressResponse> StartCircuits(Device device, IEnumerable<string> circuitIds, int durationMinutes, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
		{
			try
			{
                //prepare request
				var deviceRequest = new DeviceRequest(device);
				deviceRequest.AddCircuits(circuitIds);

                var request = new StartCircuitsRequest(deviceRequest);
                request.Body.DurationMinutes = durationMinutes; 

                //send request/get response
				var response = await this.DoRequest<StartCircuitsRequest, ProgressResponse>(request, onReconnect: onReconnect, silentMode: silentMode);

                //after-response tasks
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
