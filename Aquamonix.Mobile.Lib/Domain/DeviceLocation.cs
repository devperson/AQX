using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceLocation
	{
		[DataMember(Name = PropertyNames.Latitude)]
		public double? Latitude { get; set; }

		[DataMember(Name = PropertyNames.Longitude)]
		public double? Longitude { get; set; }
        public object Values { get; internal set; }
    }
}

