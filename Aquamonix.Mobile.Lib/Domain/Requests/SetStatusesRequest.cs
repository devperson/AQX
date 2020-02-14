using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class SetStatusesRequest : ApiRequest<SetStatusesRequestBody>
	{
		public const string RequestKey = "Req.SetSettings";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public SetStatusesRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new SetStatusesRequestBody(devices);
		}

		public SetStatusesRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new SetStatusesRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class SetStatusesRequestBody : RequestBodyWithDeviceList
	{
		public SetStatusesRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }

		/*
		 M: 
		 {
		 	Devices: {
		 		"2": {
		 			MetaData: {}
		 			StatusGroups: 
		 			{
		 				status: 
		 				{ 
		 					Statuses: 
		 					{ 
		 						ProgramDisable: 
		 						{ 
		 							Values: 
		 							{
		 								"0": { Value: 1}
		 							}
								}
							}
		 				}
					}
				}
		 	}
		 }
		 */
	}
}
