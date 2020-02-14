using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquamonix.Mobile.Lib.Services
{

    public interface IallDeviceDetails : IService
    {
        /// <summary>
        /// Gets a boolean value indicating whether or not the underlying connection is active.
        /// </summary>
		bool IsConnected { get; }
      
		//Task<DetailsResponce> RequestConnectionAsync(string username, string password, bool silentMode = false);
	
    }
    class DeviceDetailsService : ServiceBase, IallDeviceDetails
    {
        public bool IsConnected
        {
            get
            {
                return WebSocketsClient.IsConnected;
            }
        }
    }    
    
}
