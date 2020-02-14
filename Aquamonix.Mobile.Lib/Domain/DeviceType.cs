using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceType
	{
		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Settings)]
		public ItemsDictionary<DeviceSetting> Settings { get; set; }

		[DataMember(Name = PropertyNames.Features)]
		public ItemsDictionary<DeviceFeature> Features { get; set; }

		public void ReadyChildIds()
		{
			this.Settings?.ReadyDictionary();
			this.Features?.ReadyDictionary(); 
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Settings);
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Features);
		}
	}
}
