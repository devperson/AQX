using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	[DataContract]
	public class DevicesResponse : ApiResponse<DevicesResponseBody>
	{
		public DevicesResponse() : base()
		{
		}

		public DevicesResponse(Device device) : base()
		{
			this.Body = new DevicesResponseBody();
			this.Body.Devices = new ItemsDictionary<Device>();
			this.Body.Devices.Add(device.Id, device);
		}
	}

	[DataContract]
	public class DevicesResponseBody : ResponseBodyBase
	{
		[DataMember(Name=PropertyNames.Devices)]
		public ItemsDictionary<Device> Devices { get; set; }

		public override void ReadyResponse()
		{
			base.ReadyResponse();

			this.Devices?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Devices);

			if (this.Devices != null && this.Devices.Count == 1 && this.ActiveAlertsCount.GetValueOrDefault() > 0)
				this.Devices.Items.First().Value.ActiveAlertsCount = this.ActiveAlertsCount.GetValueOrDefault();
		}
	}
}
