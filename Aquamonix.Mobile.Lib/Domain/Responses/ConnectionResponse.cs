using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	[DataContract]
	public class ConnectionResponse : ApiResponse<ConnectionResponseBody>
	{
		public ConnectionResponse() : base()
		{
		}
	}

	[DataContract]
	public class ConnectionResponseBody : ResponseBodyBase
	{
		[DataMember(Name = PropertyNames.MetaData)]
		public ApplicationMetadata MetaData { get; set;}

		[DataMember(Name = PropertyNames.Devices)]
		public ItemsDictionary<Device> Devices { get; set; }

		[DataMember(Name = PropertyNames.User)]
		public UserResponseObject User { get; set; }

		[DataMember(Name = PropertyNames.AlertsCount)]
		public int? AlertsCount { get; set; }

		[DataMember(Name="CommandProgresses")]
		public CommandProgress[] CommandProgresses { get; set; }

		public override void ReadyResponse()
		{
			base.ReadyResponse();

			this.Devices?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Devices);

			if (this.MetaData != null)
				this.MetaData.ReadyChildIds();
		}
	}
}
