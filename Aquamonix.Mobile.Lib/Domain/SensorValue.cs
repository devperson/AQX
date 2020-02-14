using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class SensorValue : IDomainObjectWithId, ICloneable<SensorValue>, IMergeable<SensorValue>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Units)]
		public string Units { get; set; }

		[DataMember(Name = PropertyNames.Type)]
		public string Type { get; set; }

		[DataMember(Name = PropertyNames.Value)]
		public string Value { get; set; }

		[DataMember(Name = PropertyNames.DecimalPlaces)]
		public string DecimalPlaces { get; set; }

		[DataMember(Name = PropertyNames.TimeStamp)]
		public string TimeStamp { get; set; }

		[DataMember(Name = PropertyNames.DisplayType)]
		public string DisplayType { get; set; }

		[DataMember(Name = PropertyNames.DisplayPrecision)]
		public string DisplayPrecision { get; set; }

		[DataMember(Name = PropertyNames.EditPrecision)]
		public string EditPrecision { get; set; }

		[DataMember(Name = PropertyNames.Severity)]
		public string Severity { get; set; }


		public SensorValue Clone()
		{
			return new SensorValue()
			{
				Id = this.Id,
				Name = this.Name,
				Type = this.Type,
				Units = this.Units,
				Value = this.Value,
				DecimalPlaces = this.DecimalPlaces,
				DisplayPrecision = this.DisplayPrecision,
				DisplayType = this.DisplayType,
				EditPrecision = this.EditPrecision,
				Severity = this.Severity,
				TimeStamp = this.TimeStamp
			};
		}

		public void MergeFromParent(SensorValue parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.TimeStamp = MergeExtensions.MergeProperty(this.TimeStamp, parent.TimeStamp, removeIfMissingFromParent, parentIsMetadata);
				this.Units = MergeExtensions.MergeProperty(this.Units, parent.Units, removeIfMissingFromParent, parentIsMetadata);
				this.Type = MergeExtensions.MergeProperty(this.Type, parent.Type, removeIfMissingFromParent, parentIsMetadata);
				this.DecimalPlaces = MergeExtensions.MergeProperty(this.DecimalPlaces, parent.DecimalPlaces, removeIfMissingFromParent, parentIsMetadata);
				this.DisplayType = MergeExtensions.MergeProperty(this.DisplayType, parent.DisplayType, removeIfMissingFromParent, parentIsMetadata);
				this.DisplayPrecision = MergeExtensions.MergeProperty(this.DisplayPrecision, parent.DisplayPrecision, removeIfMissingFromParent, parentIsMetadata);
				this.EditPrecision = MergeExtensions.MergeProperty(this.EditPrecision, parent.EditPrecision, removeIfMissingFromParent, parentIsMetadata);
				this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);
				this.Severity = MergeExtensions.MergeProperty(this.Severity, parent.Severity, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public void ReadyChildIds() { }
	}
}
