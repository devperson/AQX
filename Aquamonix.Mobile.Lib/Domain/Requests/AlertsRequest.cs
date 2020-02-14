using System;
using System.Collections.Generic;	
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class AlertsRequest : ApiRequest<AlertsRequestBody>
	{
		public const string RequestKey = "Req.Alerts";

		public AlertsRequest(IEnumerable<string> deviceIds) : base(RequestKey)
		{
			this.Body = new AlertsRequestBody(deviceIds);
		}

		public AlertsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new AlertsRequestBody(devices);
		}
	}

	[DataContract]
	public class AlertsRequestBody : RequestBodyWithDeviceList
	{
		public AlertsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }
		public AlertsRequestBody(IEnumerable<string> deviceIds) : base(deviceIds) { }
	}
}
