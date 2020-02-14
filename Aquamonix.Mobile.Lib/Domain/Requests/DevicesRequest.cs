using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class DevicesRequest : ApiRequest<DevicesRequestBody>
	{
		public const string RequestKey = "Req.Devices";

		public DevicesRequest(IEnumerable<string> deviceIds) : base(RequestKey)
		{
			this.Body = new DevicesRequestBody(deviceIds);
		}

		public DevicesRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new DevicesRequestBody(devices);
		}

		public DevicesRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new DevicesRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class DevicesRequestBody : RequestBodyWithDeviceList
	{
		public DevicesRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }
		public DevicesRequestBody(IEnumerable<string> deviceIds) : base(deviceIds) { }
	}
}
