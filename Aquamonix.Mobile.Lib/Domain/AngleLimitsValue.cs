using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class AngleLimitsValue
    {
        [DataMember(Name = PropertyNames.Min)]
        public AngleMinMaxValue Min { get; set; }

        [DataMember(Name = PropertyNames.Max)]
        public AngleMinMaxValue Max { get; set; }
    }
}