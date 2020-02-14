using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class ValueVisible
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }

        [DataMember(Name = PropertyNames.Visible)]
        public bool Visible { get; set; }

        [DataMember(Name = PropertyNames.Units)]
        public string Units { get; set; }
       // public object Values { get; internal set; }
    }
}
