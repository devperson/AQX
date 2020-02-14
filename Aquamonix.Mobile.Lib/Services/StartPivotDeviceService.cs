using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.ViewModels;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;

namespace Aquamonix.Mobile.Lib.Services
{
    public interface IStartPivot : IService
    {
        Task<ProgressResponse> StartPivotDevice(DeviceDetailViewModel model, string deviceId, Action onReconnect = null, bool silentMode = false);
    }

    public class StartPivotDeviceService : ServiceBase, IStartPivot
    {
        public StartPivotDeviceService()
        {
        }

        async Task<ProgressResponse> IStartPivot.StartPivotDevice(DeviceDetailViewModel model, string deviceId, Action onReconnect, bool silentMode)
        {
            try
            {
                var request = new StartPivotDevice(model, deviceId);
                var response = await this.DoRequest<StartPivotDevice, ProgressResponse>(request, onReconnect: onReconnect, silentMode: silentMode);

                //handle after-response tasks
                ServiceContainer.InvalidateCache();


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

