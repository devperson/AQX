using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Alert : IDomainObjectWithId, ICloneable<Alert>, IMergeable<Alert>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.DeviceId)]
		public string DeviceId { get; set; }

		[DataMember(Name = PropertyNames.DateTimeUtc)]
		public string DateTimeUtc { get; set;}

		[DataMember(Name = PropertyNames.Description)]
		public string Description { get; set;}

		[DataMember(Name = PropertyNames.Active)]
		public bool Active { get; set; }

		[DataMember(Name = PropertyNames.Severity)]
		public string Severity { get; set; }

		public Alert Clone()
		{
			return new Alert()
			{
				Id = this.Id,
				Description = this.Description,
				DeviceId = this.DeviceId,
				DateTimeUtc = this.DateTimeUtc, 
				Active = this.Active, 
				Severity = this.Severity
			};
		}

		public void MergeFromParent(Alert parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.DateTimeUtc = MergeExtensions.MergeProperty(this.DateTimeUtc, parent.DateTimeUtc, removeIfMissingFromParent, parentIsMetadata);
				this.DeviceId = MergeExtensions.MergeProperty(this.DeviceId, parent.DeviceId, removeIfMissingFromParent, parentIsMetadata);
				this.Description = MergeExtensions.MergeProperty(this.Description, parent.Description, removeIfMissingFromParent, parentIsMetadata);
				this.Active = MergeExtensions.MergeProperty(this.Active, parent.Active, removeIfMissingFromParent, parentIsMetadata);
				this.Severity = MergeExtensions.MergeProperty(this.Severity, parent.Severity, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public void UpdateFrom(Alert alert)
		{
			if (alert != null)
			{
				this.Description = alert.Description;
				this.DateTimeUtc = alert.DateTimeUtc;
				this.Active = alert.Active;
				this.Severity = alert.Severity;
			}
		}

		public void ReadyChildIds() { }
	}
}
