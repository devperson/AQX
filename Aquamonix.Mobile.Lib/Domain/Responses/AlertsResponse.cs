using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	[DataContract]
	public class AlertsResponse : ApiResponse<AlertsResponseBody>
	{
		public AlertsResponse() : base()
		{
		}
	}

	[DataContract]
	public class AlertsResponseBody : ResponseBodyBase
	{
	}
}
