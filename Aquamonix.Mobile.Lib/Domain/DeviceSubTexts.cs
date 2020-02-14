using System;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class DeviceSubTexts 
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }
}
