using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class SettingsRequest : ApiRequest<SettingsRequestBody>
	{
		public const string RequestKey = "Req.Settings";

		public SettingsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new SettingsRequestBody(devices);
		}

		public SettingsRequest(IEnumerable<string> deviceIds) : base(RequestKey)
		{
			this.Body = new SettingsRequestBody(deviceIds);
		}

		public SettingsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new SettingsRequestBody(new DeviceRequest[]{ device });
		}
	}

	[DataContract]
	public class SettingsRequestBody : RequestBodyWithDeviceList
	{
		public SettingsRequestBody(IEnumerable<string> deviceIds) : base(deviceIds) { }
		public SettingsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }
	}
}
