using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class UserResponseObject
	{
		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.DevicesAccess)]
		public ItemsDictionary<DeviceAccess> DevicesAccess { get; set; }
	}
}

