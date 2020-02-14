using System;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Schedule : IDomainObjectWithId, ICloneable<Schedule>, IMergeable<Schedule>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.SetToRun)]
		public bool SetToRun { get; set;}

		[DataMember(Name = PropertyNames.OrganisationId)]
		public string OrganisationId { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.StartTimeInMinutes)]
		public int StartTimeInMinutes { get; set; }

		[DataMember(Name = PropertyNames.EndTimeInMinutes)]
		public int EndTimeInMinutes { get; set; }

		[DataMember(Name = PropertyNames.SubTexts)]
		public ItemsDictionary<SummaryValue> SubTexts { get; set; }

		public Schedule Clone()
		{
			var clone = new Schedule()
			{
				Id = this.Id,
				EndTimeInMinutes = this.EndTimeInMinutes,
				SetToRun = this.SetToRun,
				StartTimeInMinutes = this.StartTimeInMinutes,
				OrganisationId = this.OrganisationId, 
				Name = this.Name
			};

			if (this.SubTexts != null)
				clone.SubTexts = this.SubTexts.Clone();

			return clone;
		}

		public void MergeFromParent(Schedule parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.SetToRun = MergeExtensions.MergeProperty(this.SetToRun, parent.SetToRun, removeIfMissingFromParent, parentIsMetadata);
				this.OrganisationId = MergeExtensions.MergeProperty(this.OrganisationId, parent.OrganisationId, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.StartTimeInMinutes = MergeExtensions.MergeProperty(this.StartTimeInMinutes, parent.StartTimeInMinutes, removeIfMissingFromParent, parentIsMetadata);
				this.EndTimeInMinutes = MergeExtensions.MergeProperty(this.EndTimeInMinutes, parent.EndTimeInMinutes, removeIfMissingFromParent, parentIsMetadata);
				this.SubTexts = ItemsDictionary<SummaryValue>.MergePropertyLists(this.SubTexts, parent.SubTexts, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public void ReadyChildIds() { }
	}
}
