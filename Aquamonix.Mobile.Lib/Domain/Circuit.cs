using System;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Circuit : IDomainObjectWithId, ICloneable<Circuit>, IMergeable<Circuit>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name=PropertyNames.ScheduleState)]
		public bool ScheduleState { get; set;}

		[DataMember(Name = PropertyNames.TestState)]
		public bool TestState { get; set; }

		[DataMember(Name = PropertyNames.ActiveState)]
		public bool ActiveState { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.KwhMeter)]
		public string KwhMeter { get; set; }

		[IgnoreDataMember]
		public bool Starting { get; set; }

		[IgnoreDataMember]
		public bool Selected { get; set; }

		//TODO:* how to figure out if it's running? 
		[IgnoreDataMember]
		public bool IsRunning
		{
			get
			{
				return (this.TestState);
			}
		}

		public Circuit Clone()
		{
			return new Circuit()
			{
				Id = this.Id,
				ActiveState = this.ActiveState,
				ScheduleState = this.ScheduleState,
				TestState = this.TestState,
				Name = this.Name, 
				KwhMeter = this.KwhMeter, 
				Selected = this.Selected, 
				Starting =this.Starting
			}; 
		}

		public void MergeFromParent(Circuit parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.ScheduleState = MergeExtensions.MergeProperty(this.ScheduleState, parent.ScheduleState, removeIfMissingFromParent, parentIsMetadata);
				this.TestState = MergeExtensions.MergeProperty(this.TestState, parent.TestState, removeIfMissingFromParent, parentIsMetadata);
				this.ActiveState = MergeExtensions.MergeProperty(this.ActiveState, parent.ActiveState, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.KwhMeter = MergeExtensions.MergeProperty(this.KwhMeter, parent.KwhMeter, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public void ReadyChildIds() { }
	}
}
