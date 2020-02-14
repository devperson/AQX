using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class TestStationsRequest : ApiRequest<TestStationsRequestBody>
	{
		public const string RequestKey = "Req.TestStations";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public TestStationsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new TestStationsRequestBody(devices);
		}

		public TestStationsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new TestStationsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class TestStationsRequestBody : RequestBodyWithDeviceList
	{
		public TestStationsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }
	}
}
