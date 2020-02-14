using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class FeaturesPrograms
    {
        [DataMember(Name = PropertyNames.NumberOfRepeats)]
        public int NumberOfRepeats { get; set; }

        [DataMember(Name = PropertyNames.RepeatNumber)]
        public int RepeatNumber { get; set; }

        [DataMember(Name = PropertyNames.Steps)]
        public ItemsDictionary<FeatureProgramsSteps> Steps { get; set; }
    }
}
