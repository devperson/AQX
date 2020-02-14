using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class ApplicationMetadata
	{
		[DataMember(Name = PropertyNames.MetaDataTimeStamp)]
		public int TimeStamp { get; set; }

		[DataMember(Name = PropertyNames.Retic)]
		public Retic Retic { get; set;}

		[DataMember(Name = PropertyNames.Badges)]
		public ItemsDictionary<DeviceBadge> Badges { get; set;}

		[DataMember(Name = PropertyNames.DeviceTypes)]
		public ItemsDictionary<DeviceType> DeviceTypes { get; set; }

		public void ReadyChildIds()
		{
			if (this.DeviceTypes != null)
			{
				foreach (var devType in this.DeviceTypes.Values)
					devType.ReadyChildIds();
			}
		}

		public string GetFriendlyNameForDeviceType(string deviceType)
		{
			string output = deviceType;

			ExceptionUtility.Try(() =>
			{
				if (deviceType != null)
				{
					if (this.DeviceTypes != null && this.DeviceTypes.ContainsKey(deviceType))
					{
						var devType = this.DeviceTypes[deviceType];
						if (devType != null)
						{
							output = devType.Name;
						}
					}
				}
			});

			return output;
		}

		public SeverityLevel GetSeverityLevel(int value)
		{
			SeverityLevel output = SeverityLevel.Low;

			Enum.TryParse<SeverityLevel>(value.ToString(), out output);

			return output;
		}

		public int? GetColorForZone(string zoneId)
		{
			int? output = null;

			ExceptionUtility.Try(() =>
			{
				if (zoneId != null)
				{
					if (this.Retic?.Zones != null)
					{
						if (this.Retic.Zones.ContainsKey(zoneId))
						{
							var zone = this.Retic.Zones[zoneId];
							output = zone.Colour;
						}
					}
				}
			});

			return output;
		}
    }

	[DataContract]
	public class Retic
	{
		[DataMember(Name = PropertyNames.Zones)]
		public ItemsDictionary<DeviceZone> Zones { get; set; }
  	}
}
