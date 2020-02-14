using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class StartProgramsRequest : ApiRequest<StartProgramsRequestBody>
	{
		public const string RequestKey = "Req.StartPrograms";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public StartProgramsRequest(IEnumerable<DeviceRequest> devices) : base(RequestKey)
		{
			this.Body = new StartProgramsRequestBody(devices);
		}

		public StartProgramsRequest(DeviceRequest device) : base(RequestKey)
		{
			this.Body = new StartProgramsRequestBody(new DeviceRequest[] { device });
		}
	}

	[DataContract]
	public class StartProgramsRequestBody : RequestBodyWithDeviceList
	{
		public StartProgramsRequestBody(IEnumerable<DeviceRequest> devices) : base(devices) { }


    }
}
