using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class NextIrrigationsRequest : ApiRequest<NextIrrigationsRequestBody>
	{
		public const string RequestKey = "Req.NextIrrigations";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public NextIrrigationsRequest(IEnumerable<string> deviceIds) : base(RequestKey)
		{
			this.Body = new NextIrrigationsRequestBody(deviceIds);
		}

		public NextIrrigationsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new NextIrrigationsRequestBody(devices);
		}

		public NextIrrigationsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new NextIrrigationsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class NextIrrigationsRequestBody : RequestBodyWithDeviceList
	{
		public NextIrrigationsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }

		public NextIrrigationsRequestBody(IEnumerable<string> deviceIds) : base(deviceIds) { }
	}
}
