using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	[DataContract]
	public class DeviceBriefsResponse : ApiResponse<DeviceBriefsResponseBody>
	{
		public DeviceBriefsResponse() : base()
		{
		}
	}

	[DataContract]
	public class DeviceBriefsResponseBody : ResponseBodyBase
	{
		[DataMember(Name = PropertyNames.Devices)]
		public ItemsDictionary<Device> Devices { get; set; }

		[DataMember(Name = PropertyNames.AlertsCount)]
		public int? AlertsCount { get; set; }

		public override void ReadyResponse()
		{
			base.ReadyResponse();

			this.Devices?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Devices);
		}
	}
}
