using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceStatus : ICloneable<DeviceStatus>, IMergeable<DeviceStatus>
	{
		[DataMember(Name = PropertyNames.Value)]
		public string Value { get; set; }

		[DataMember(Name = PropertyNames.Severity)]
		public string Severity { get; set; }

		[DataMember(Name = PropertyNames.UpdatedDateUtc)]
		public string UpdatedDateUtc { get; set; }

		public DeviceStatus Clone()
		{
			return new DeviceStatus() { Severity = this.Severity, UpdatedDateUtc = this.UpdatedDateUtc, Value = this.Value };
		}

		public void MergeFromParent(DeviceStatus parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);
				this.Severity = MergeExtensions.MergeProperty(this.Severity, parent.Severity, removeIfMissingFromParent, parentIsMetadata);
				this.UpdatedDateUtc = MergeExtensions.MergeProperty(this.UpdatedDateUtc, parent.UpdatedDateUtc, removeIfMissingFromParent, parentIsMetadata);
			}
		}
	}
}