using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class DeviceBriefsRequest : ApiRequest<DevicesRequestBody>
	{
		public const string RequestKey = "Req.DeviceBriefs";

		public DeviceBriefsRequest(IEnumerable<string> deviceIds) : base(RequestKey)
		{
			this.Body = new DevicesRequestBody(deviceIds);
		}

		public DeviceBriefsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new DevicesRequestBody(devices);
		}

		public DeviceBriefsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new DevicesRequestBody(new DeviceRequest[] { device });
		}
	}
}
