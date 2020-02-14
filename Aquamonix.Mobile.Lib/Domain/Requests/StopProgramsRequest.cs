using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class StopProgramsRequest : ApiRequest<StopProgramsRequestBody>
	{
		public const string RequestKey = "Req.StopPrograms";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public StopProgramsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new StopProgramsRequestBody(devices);
		}

		public StopProgramsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new StopProgramsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class StopProgramsRequestBody : RequestBodyWithDeviceList
	{
		public StopProgramsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }
	}
}
