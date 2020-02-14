using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class StepsVisibleValues
    {
        [DataMember(Name = PropertyNames.Visible)]
        public bool Visible { get; set; }

        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }
}
