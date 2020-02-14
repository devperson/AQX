using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class TitleDictionary
    {
        [DataMember(Name = PropertyNames.Title)]
        public IEnumerable<string> Title { get; set; }
    }
}
