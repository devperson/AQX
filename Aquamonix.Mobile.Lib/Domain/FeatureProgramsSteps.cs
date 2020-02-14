using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class FeatureProgramsSteps
    {
        [DataMember(Name = PropertyNames.Auxiliary1)]
        public FeatureProgramsStepsDictionary Auxiliary1 { get; set; }

        [DataMember(Name = PropertyNames.Auxiliary2)]
        public FeatureProgramsStepsDictionary Auxiliary2 { get; set; }

        [DataMember (Name = PropertyNames.EndGun)]
        public FeatureProgramsStepsDictionary EndGun { get; set; }
       
        [DataMember(Name = PropertyNames.Speed)]
        public UnitValue Speed {get;set; }
    }
}
