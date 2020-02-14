using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class StartStationsRequest : ApiRequest<StartStationsRequestBody>
	{
		public const string RequestKey = "Req.StartStations";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public StartStationsRequest(IEnumerable<DeviceRequest> devices, int durationMinutes) : base(RequestKey)
		{
			this.Body = new StartStationsRequestBody(devices, durationMinutes);
		}

		public StartStationsRequest(DeviceRequest device, int durationMinutes) : base(RequestKey)
		{
			this.Body = new StartStationsRequestBody(new DeviceRequest[] { device }, durationMinutes);
		}
	}

	[DataContract]
	public class StartStationsRequestBody : RequestBodyWithDeviceList
	{
		[DataMember(Name = PropertyNames.DurationMinutes)]
		public int DurationMinutes { get; set; }

		//[DataMember(Name=PropertyNames.DurationEndTimeUtc)]
		//public string DurationEndTimeUtc { get; set;}

		public StartStationsRequestBody(IEnumerable<DeviceRequest> devices, int durationMinutes) : base(devices)
		{
			//this.DurationEndTimeUtc = DateTimeUtil.ToUtcDateString(DateTime.Now.AddMinutes(durationMinutes)); 
			this.DurationMinutes = durationMinutes;
		}
	}
}
