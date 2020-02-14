using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class SensorsRequest : ApiRequest<SensorsRequestBody>
	{
		public const string RequestKey = "Req.Sensors";

		public SensorsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new SensorsRequestBody(devices);
		}

		public SensorsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new SensorsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class SensorsRequestBody : RequestBodyWithDeviceList
	{
		public SensorsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }
	}
}
