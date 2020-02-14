using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class StartCircuitsRequest : ApiRequest<StartCircuitsRequestBody>
	{
		public const string RequestKey = "Req.TestCircuits";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

		public StartCircuitsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new StartCircuitsRequestBody(devices);
		}

		public StartCircuitsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new StartCircuitsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class StartCircuitsRequestBody : RequestBodyWithDeviceList
	{
		[DataMember(Name = PropertyNames.DurationMinutes)]
		public int DurationMinutes { get; set; }

		public StartCircuitsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }
	}
}
