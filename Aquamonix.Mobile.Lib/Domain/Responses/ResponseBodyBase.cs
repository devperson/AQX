using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	[DataContract]
	public class ResponseBodyBase
	{
		[DataMember(Name=PropertyNames.ResponseCode)]
		public int? ResponseCode { get; set; }

		[DataMember(Name = PropertyNames.IsAuthenticated)]
		public bool? IsAuthenticated { get; set; }

		[DataMember(Name = PropertyNames.SessionId)]
		public string SessionId { get; set; }

		[DataMember(Name = PropertyNames.CommandId)]
		public string CommandId { get; set; }

		[DataMember(Name = PropertyNames.ServerVersion)]
		public string ServerVersion { get; set; }

		[DataMember(Name = PropertyNames.Alerts)]
		public ItemsDictionary<Alert> Alerts { get; set; }

		public int? ActiveAlertsCount { get; private set;}

		public virtual void ReadyResponse()
		{
			this.Alerts?.ReadyDictionary();

			if (this.Alerts != null)
			{
				//count up active alerts & update count 
				int count = 0;
				foreach (var alert in this.Alerts)
				{
					if (alert.Value.Active)
						count++;
				}

				this.ActiveAlertsCount = count;
			}
		}
	}
}

