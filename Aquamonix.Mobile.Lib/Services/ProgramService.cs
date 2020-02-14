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
    /// Interface to program service.
    /// </summary>
	public interface IProgramService : IService
    {
        /// <summary>
        /// Starts a program.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to start program</param>
        /// <param name="programId">ID of program to start</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
		Task<ProgressResponse> StartProgram(Device device, string programId, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);


        Task<ProgressResponse> StartPivotProgram(Device device, string programId,int NoOfrepeats, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);
        /// <summary>
        /// Stops a program.
        /// This call invalidates the cache.
        /// </summary>
        /// <param name="device">The device for which to stop program</param>
        /// <param name="programId">ID of program to stop</param>
        /// <param name="handleUpdates">Handler/callback for progress updates</param>
        /// <param name="onReconnect">What to do if the connection is interrupted & restored</param>
        /// <param name="silentMode">If true, show no alerts or popups for any reason</param>
        /// <returns>Response from the server</returns>
        //invalidates cache 
        Task<ProgressResponse> StopProgram(Device device, string programId, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false);
        Task<object> StartPivotProgram(object device, string id, Action<ProgressResponse> p);
    }

    /// <summary>
    /// Implementation of program service.
    /// </summary>
	public class ProgramService : ServiceBase, IProgramService
    {
        public async Task<ProgressResponse> StartProgram(Device device, string programId, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
        {
            try
            {
                //create/configure request
                var deviceRequest = new DeviceRequest(device);
                deviceRequest.AddProgram(programId);

                //send & get response
                var request = new StartProgramsRequest(deviceRequest);
                var response = await this.DoRequest<StartProgramsRequest, ProgressResponse>(request, onReconnect: onReconnect, silentMode: silentMode);

                //after-call maintenance
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

        public async Task<ProgressResponse> StartPivotProgram(Device device, string programId,int NoofRepeats, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
        {
            try
            {
                //create/configure request
                var deviceRequest = new DeviceRequest(device);
                deviceRequest.AddProgram(programId);

                //send & get response
                var request = new StartPivotProgramsRequest(deviceRequest, programId,NoofRepeats);
                var response = await this.DoRequest<StartPivotProgramsRequest, ProgressResponse>(request, onReconnect: onReconnect, silentMode: silentMode);

                //after-call maintenance
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
         public async Task<ProgressResponse> StopProgram(Device device, string programId, Action<ProgressResponse> handleUpdates = null, Action onReconnect = null, bool silentMode = false)
        {
            try
            {
                //create/configure request
                var deviceRequest = new DeviceRequest(device);
                deviceRequest.AddProgram(programId);

                //send & get response
                var request = new StopProgramsRequest(deviceRequest);
                var response = await this.DoRequest<StopProgramsRequest, ProgressResponse>(request, onReconnect: onReconnect, silentMode: silentMode);

                //after-call maintenance
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

        public Task<object> StartPivotProgram(object device, string id, Action<ProgressResponse> p)
        {
            throw new NotImplementedException();
        }
    }
}
