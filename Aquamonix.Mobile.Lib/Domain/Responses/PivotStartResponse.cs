using System;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
    [DataContract]
    public class PivotStartResponse : ApiResponse<PivotResponseBody>
    {
        public PivotStartResponse()
        {
        }
    }

    [DataContract]
    public class PivotResponseBody : ResponseBodyBase
    {
        [DataMember(Name = PropertyNames.Progress)]
        public string Progress { get; set; }

        [DataMember(Name = PropertyNames.ProgressSpecific)]
        public string ProgressSpecific { get; set; }

        [DataMember(Name = PropertyNames.ProgressSpecificDescription)]
        public string ProgressSpecificDescription { get; set; }

        [DataMember(Name = PropertyNames.ExpireTimeUtc)]
        public string ExpireTimeUtc { get; set; }

        [DataMember(Name = PropertyNames.UpdatedDateUtc)]
        public string UpdatedDateUtc { get; set; }
    }
}