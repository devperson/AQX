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
    /// Interface to stations service.
    /// </summary>
	public interface IStationService : IService
    {
        /// <summary>
        /// Starts a given set of stations.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to start the stations</param>
        /// <param name="stationIds">List of ids of stations to start</param>
        /// <param name="pumpIds">List of ids of pumps to start</param>
        /// <param name="durationMinutes">Number of minutes for which to run</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> StartStations(Device device, IEnumerable<string> stationIds, IEnumerable<string> pumpIds, int durationMinutes, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);


        /// <summary>
        /// Tests a given set of stations.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to test the stations</param>
        /// <param name="stationIds">List of ids of stations to start</param>
        /// <param name="durationMinutes">Number of minutes for which to run</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> TestStations(Device device, IEnumerable<string> stationIds, int durationMinutes, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);
	}

    /// <summary>
    /// Implementation of stations service.
    /// </summary>
	public class StationService : ServiceBase, IStationService
	{
		public async Task<ProgressResponse> StartStations(Device device, IEnumerable<string> stationIds, IEnumerable<string> pumpIds, int durationMinutes, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
		{
			try
			{
                //create & configure request
				var deviceRequest = new DeviceRequest(device);
				deviceRequest.AddStations(stationIds);

				if (pumpIds != null)
					deviceRequest.AddPumps(pumpIds);

                //send request/get response
				var request = new StartStationsRequest(deviceRequest, durationMinutes);
				var response = await this.DoRequest<StartStationsRequest, ProgressResponse>(request, onReconnect:onReconnect, silentMode: silentMode);

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

		public async Task<ProgressResponse> TestStations(Device device, IEnumerable<string> stationIds, int durationMinutes, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
		{
			try
            {
                //create & configure request
				var deviceRequest = new DeviceRequest(device);
				deviceRequest.AddStations(stationIds);

                //send request/get response
				var request = new TestStationsRequest(deviceRequest);
				var response = await this.DoRequest<TestStationsRequest, ProgressResponse>(request, onReconnect:onReconnect, silentMode: silentMode);

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
