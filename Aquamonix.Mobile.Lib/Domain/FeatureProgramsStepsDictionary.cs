using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class FeatureProgramsStepsDictionary
    {
        [DataMember(Name = PropertyNames.Dictionary)]
        public TitleDictionary Dictionary { get; set; }
    }
}
