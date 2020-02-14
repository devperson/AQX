using Aquamonix.Mobile.Lib.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class DeviceLengthValue
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }
}
