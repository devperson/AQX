using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class AngleMinMaxValue
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }

    }
}
