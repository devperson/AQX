
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceBadge : IMergeable<DeviceBadge>, ICloneable<DeviceBadge>
	{
		[DataMember(Name = PropertyNames.Type)]
		public string Type { get; set; }

		[DataMember(Name = PropertyNames.Texts)]
		public string[] Texts { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Severity)]
		public string Severity { get; set; }


		public void MergeFromParent(DeviceBadge parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Type = MergeExtensions.MergeProperty(this.Type, parent.Type, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.Texts = MergeExtensions.MergeProperty(this.Texts, parent.Texts, removeIfMissingFromParent, parentIsMetadata);
				this.Severity = MergeExtensions.MergeProperty(this.Severity, parent.Severity, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public DeviceBadge Clone()
		{
			return new DeviceBadge()
			{
				Type = this.Type,
				Name = this.Name,
				Severity = this.Severity,
				Texts = this.Texts
			};
		}
	}
}