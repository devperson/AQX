using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class UnitValue
    {
        [DataMember(Name = PropertyNames.Units)]
        public string Units { get; set; }

        [DataMember(Name = PropertyNames.Type)]
        public string Type { get; set; }

    }
}
