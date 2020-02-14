using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceMetadata : ICloneable<DeviceMetadata>
	{
		[DataMember(Name = PropertyNames.MetaDataTimeStamp)]
		public int TimeStamp { get; set; }

		[DataMember(Name = PropertyNames.Device)]
		public Device Device { get; set;}

		public void ReadyChildIds()
		{
			if (this.Device != null)
				this.Device.ReadyChildIds();
		}

		public DeviceMetadata Clone()
		{
			var clone = new DeviceMetadata() { TimeStamp = this.TimeStamp };

			if (this.Device != null)
				clone.Device = this.Device.Clone();

			return clone;
		}
	}
}
