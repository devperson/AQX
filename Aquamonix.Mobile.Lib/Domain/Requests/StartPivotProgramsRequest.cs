using Aquamonix.Mobile.Lib.Domain.Requests;
using Aquamonix.Mobile.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
	[DataContract]
	public class StartPivotProgramsRequest : ApiRequestForPivotPrograms<StartPivotProgramsRequestBody>
	{
		public const string RequestKey = "Req.StartPivot";

        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public StartPivotProgramsRequest(DeviceRequest devices ,string programId, int noofrepeats) : base(RequestKey)
		{
            this.Body = new StartPivotProgramsRequestBody(devices,devices.Id,programId,noofrepeats);
		}
	}

    [DataContract]
    public class StartPivotProgramsRequestBody
    {
        [DataMember(Name = PropertyNames.DeviceId)]
        public string DeviceId { get; set; }
        [DataMember(Name = PropertyNames.Programs)]
        public Programs Programs { get; private set; }


        public StartPivotProgramsRequestBody(DeviceRequest model,string deviceId, string programId, int NoofRepeats)
        {  
           
            this.DeviceId = deviceId;
            var program = new Program();
            program.NumberOfRepeats = NoofRepeats;
            Programs = new Programs();
            Programs.Add(programId, program);
            //Programs.CurrentProgramId = programId;
        }
    }
}