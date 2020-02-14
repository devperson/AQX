using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceSetting : IDomainObjectWithId, ICloneable<DeviceSetting>, IMergeable<DeviceSetting>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Editable)]
		public bool? Editable { get; set; }

		[DataMember(Name = PropertyNames.Updatable)]
		public bool? Updatable { get; set; }

		[DataMember(Name = PropertyNames.Value)]
		public string Value { get; set;}

		[DataMember(Name = PropertyNames.Values)]
		public ItemsDictionary<DeviceSettingValue> Values { get; set; }

		public DeviceSetting Clone() 
		{
			var clone = new DeviceSetting()
			{
				Id = this.Id,
				Name = this.Name,
				Editable = this.Editable,
				Updatable = this.Updatable,
				Value = this.Value
			};

			if (this.Values != null)
				clone.Values = this.Values.Clone();

			return clone;
		}

		public void MergeFromParent(DeviceSetting parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.Editable = MergeExtensions.MergeProperty(this.Editable, parent.Editable, removeIfMissingFromParent, parentIsMetadata);
				this.Updatable = MergeExtensions.MergeProperty(this.Updatable, parent.Updatable, removeIfMissingFromParent, parentIsMetadata);
				this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);

				this.Values = ItemsDictionary<DeviceSettingValue>.MergePropertyLists(this.Values, parent.Values, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public string GetValue(out DeviceSettingValue settingValue)
		{
			settingValue = null;
			var properValue = this.Values?.Values?.FirstOrDefault();
			if (properValue != null)
			{
				settingValue = properValue;
				return properValue.Value;
			}

			return this.Value;
		}

		public string GetValue()
		{
			DeviceSettingValue value;
			return this.GetValue(out value);
		}

		public void ReadyChildIds() { }
	}

	[DataContract]
	public class DeviceSettingValue : ICloneable<DeviceSettingValue>, IMergeable<DeviceSettingValue>
	{
       
        [DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Value)]
		public string Value { get; set; }

		[DataMember(Name = PropertyNames.Type)]
		public string Type { get; set; }

		[DataMember(Name = PropertyNames.DisplayType)]
		public string DisplayType { get; set; }

		[DataMember(Name = PropertyNames.Units)]
		public string Units { get; set; }

		[DataMember(Name = PropertyNames.DisplayPrecision)]
		public string DisplayPrecision { get; set; }

		[DataMember(Name = PropertyNames.EditPrecision)]
		public string EditPrecision { get; set; }

		[DataMember(Name = PropertyNames.Title)]
		public string Title { get; set; }

		[DataMember(Name = PropertyNames.SubText)]
		public string[] SubText { get; set; }

		[DataMember(Name = PropertyNames.Validation)]
		public MinMax Validation { get; set; }

		[DataMember(Name = PropertyNames.Dictionary)]
		public SettingValueDictionary Dictionary { get; set; }

		[DataMember(Name = PropertyNames.Enum)]
		public ItemsDictionary<DeviceSettingValue> Enum { get; set; }

		public DeviceSettingValue Clone()
		{
			var clone = new DeviceSettingValue()
			{
				Name = this.Name,
				DisplayType = this.DisplayType,
				DisplayPrecision = this.DisplayPrecision,
				EditPrecision = this.EditPrecision,
				Title = this.Title,
				Type = this.Type,
				Units = this.Units,
				Value = this.Value,
				SubText = this.SubText
			};

			if (this.Validation != null)
				clone.Validation = this.Validation.Clone();

			if (this.Dictionary != null)
				clone.Dictionary = this.Dictionary.Clone();

			if (this.Enum != null)
				clone.Enum = this.Enum.Clone();

			return clone;
		}

		public void MergeFromParent(DeviceSettingValue parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);
				this.Type = MergeExtensions.MergeProperty(this.Type, parent.Type, removeIfMissingFromParent, parentIsMetadata);
				this.DisplayType = MergeExtensions.MergeProperty(this.DisplayType, parent.DisplayType, removeIfMissingFromParent, parentIsMetadata);
				this.DisplayPrecision = MergeExtensions.MergeProperty(this.DisplayPrecision, parent.DisplayPrecision, removeIfMissingFromParent, parentIsMetadata);
				this.EditPrecision = MergeExtensions.MergeProperty(this.EditPrecision, parent.EditPrecision, removeIfMissingFromParent, parentIsMetadata);
				this.Units = MergeExtensions.MergeProperty(this.Units, parent.Units, removeIfMissingFromParent, parentIsMetadata);
				this.Title = MergeExtensions.MergeProperty(this.Title, parent.Title, removeIfMissingFromParent, parentIsMetadata);
				this.SubText = MergeExtensions.MergeProperty(this.SubText, parent.SubText, removeIfMissingFromParent, parentIsMetadata);

				//TODO: should these properties be merged? 
				if (this.Validation == null && parent.Validation != null)
					this.Validation = parent.Validation.Clone();

				if (this.Dictionary == null && parent.Dictionary != null)
					this.Dictionary = parent.Dictionary.Clone();

				this.Enum = ItemsDictionary<DeviceSettingValue>.MergePropertyLists(this.Enum, parent.Enum, removeIfMissingFromParent, parentIsMetadata);
			}
		}
	}
}
