using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class StopIrrigationsRequest : ApiRequest<StopIrrigationsRequestBody>
	{
		public const string RequestKey = "Req.StopIrrigations";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public StopIrrigationsRequest(IEnumerable<string> deviceIds) : base(RequestKey)
		{
			this.Body = new StopIrrigationsRequestBody(deviceIds);
		}

		public StopIrrigationsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new StopIrrigationsRequestBody(devices);
		}

		public StopIrrigationsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new StopIrrigationsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class StopIrrigationsRequestBody : RequestBodyWithDeviceList
	{
		public StopIrrigationsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }

		public StopIrrigationsRequestBody(IEnumerable<string> deviceIds) : base(deviceIds) { }
	}
}
