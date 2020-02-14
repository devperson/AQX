using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class DeviceFeature : IDomainObjectWithId, ICloneable<DeviceFeature>, IMergeable<DeviceFeature>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Type)]
		public string Type { get; set; }


		//[DataMember(Name = PropertyNames.Value)]
		//public string Value { get; set; }

		[DataMember(Name = PropertyNames.Variation)]
		public string Variation { get; set; }

		[DataMember(Name = PropertyNames.Setting)]
		public string SettingName { get; set; }

		[DataMember(Name = PropertyNames.Dictionary)]
		public DeviceFeatureDictionary Dictionary { get; set; }

		//TODO: make sure this is used everywhere it's needed. Whats the difference between Editable and Updatable? 
		[DataMember(Name = PropertyNames.Updatable)]
		public bool? Updatable { get; set; }

		[DataMember(Name = PropertyNames.SensorGroups)]
		public ItemsDictionary<SensorGroup> SensorGroups { get; set; }


        #region For Pivot

        [DataMember(Name = PropertyNames.Program)]
        public FeaturesPrograms Program { get; set; }

        #endregion


        [IgnoreDataMember]
		public DeviceSetting Setting { get; set; }

        public string Unit { get; set; }

		public DeviceFeature Clone()
		{
			var clone = new DeviceFeature()
			{
				Id = this.Id,
				Type = this.Type,
				SettingName = this.SettingName,
				Updatable = this.Updatable,
				Variation = this.Variation               
			};

			if (this.SensorGroups != null)
				clone.SensorGroups = this.SensorGroups.Clone();

			if (this.Dictionary != null)
				clone.Dictionary = this.Dictionary.Clone();

			if (this.Setting != null)
				clone.Setting = this.Setting.Clone();

			return clone;
		}

		public void MergeFromParent(DeviceFeature parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Type = MergeExtensions.MergeProperty(this.Type, parent.Type, removeIfMissingFromParent, parentIsMetadata);
				this.Variation = MergeExtensions.MergeProperty(this.Variation, parent.Variation, removeIfMissingFromParent, parentIsMetadata);
				this.SettingName = MergeExtensions.MergeProperty(this.SettingName, parent.SettingName, removeIfMissingFromParent, parentIsMetadata);
				this.Updatable = MergeExtensions.MergeProperty(this.Updatable, parent.Updatable, removeIfMissingFromParent, parentIsMetadata);

				//sensor groups 
				this.SensorGroups = ItemsDictionary<SensorGroup>.MergePropertyLists(this.SensorGroups, parent.SensorGroups, removeIfMissingFromParent, parentIsMetadata);

				//dictionary 
				if (this.Dictionary == null && parent.Dictionary != null)
					this.Dictionary = new DeviceFeatureDictionary();

				if (this.Dictionary != null)
					this.Dictionary.MergeFromParent(parent.Dictionary, removeIfMissingFromParent, parentIsMetadata);

				//setting 
				if (this.Setting == null && parent.Setting != null)
					this.Setting = new DeviceSetting();

				if (this.Setting != null)
					this.Setting.MergeFromParent(parent.Setting, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public void ReadyChildIds() { }
  	}
}
