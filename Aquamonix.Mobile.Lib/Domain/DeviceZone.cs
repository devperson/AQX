using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceZone
	{
		[DataMember(Name = PropertyNames.Colour)]
		public int Colour { get; set; }
	}
}
