using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class PrevIrrigationsRequest : ApiRequest<PrevIrrigationsRequestBody>
	{
		public const string RequestKey = "Req.PrevIrrigations";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public PrevIrrigationsRequest(IEnumerable<string> deviceIds) : base(RequestKey)
		{
			this.Body = new PrevIrrigationsRequestBody(deviceIds);
		}

		public PrevIrrigationsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new PrevIrrigationsRequestBody(devices);
		}

		public PrevIrrigationsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new PrevIrrigationsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class PrevIrrigationsRequestBody : RequestBodyWithDeviceList
	{
		public PrevIrrigationsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }

		public PrevIrrigationsRequestBody(IEnumerable<string> deviceIds) : base(deviceIds) { }
	}
}
