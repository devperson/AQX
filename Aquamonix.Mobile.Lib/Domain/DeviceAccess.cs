using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceAccess
	{
		[DataMember(Name = PropertyNames.AccessLevel)]
		public string AccessLevel { get; set; }
        
        //added
        [DataMember(Name = PropertyNames.AlertsAccessLevel)]
        public string AlertsAccessLevel { get; set; }
    }
}
