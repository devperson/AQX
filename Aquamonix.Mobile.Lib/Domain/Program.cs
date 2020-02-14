using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Program : IDomainObjectWithId, ICloneable<Program>, IMergeable<Program>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

        [DataMember(Name = PropertyNames.Status)]
        public Status Status { get; set; }

        [DataMember(Name = PropertyNames.SetToRun)]
		public bool? SetToRun { get; set; }

		[DataMember(Name = PropertyNames.StartTimes)]
		public ItemsDictionary<StartTime> StartTimes { get; set; }

		[DataMember(Name = PropertyNames.DayTable)]
		public int[] DayTable { get; set;}

		[DataMember(Name = PropertyNames.StationGroups)]
		public ItemsDictionary<StationGroup> StationGroups { get; set;}

		[DataMember(Name = PropertyNames.IsUpdatingStatus)]
		public bool IsUpdatingStatus { get; set; }

        #region For Pivot

        [DataMember(Name = PropertyNames.CurrentProgramId)]
        public int CurrentProgramId { get; set; }

        [DataMember(Name = PropertyNames.CurrentStepId)]
        public string CurrentStepId { get; set; }

        [DataMember(Name = PropertyNames.NumberOfRepeats)]
        public int NumberOfRepeats { get; set; }

        [DataMember(Name = PropertyNames.RepeatNumber)]
        public int RepeatNumber { get; set; }

        [DataMember(Name = PropertyNames.Steps)]
        public ItemsDictionary<StepsItems> Steps { get; set; }

        [DataMember(Name = PropertyNames.Type)]
        public string Type { get; set; }
        #endregion

        //TODO: find out all possible values for the status value property 
        //HARDCODED
        [IgnoreDataMember]
		public bool IsRunning
		{
			get
			{
				return (this.Status?.Value == "Running");
			}
		}

		[IgnoreDataMember]
		public bool Starting
		{
			get; set;
		}


		public void ReadyChildIds()
		{
			this.StationGroups?.ReadyDictionary();
			this.StartTimes?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.StationGroups);
			//DomainObjectWithId.ReadyChildIdsDictionary(this.StartTimes);
		}

		public Program Clone()
		{
			var clone = new Program()
			{
				Id = this.Id,
				Name = this.Name,
				SetToRun = this.SetToRun
			};

			if (this.DayTable != null)
			{
				clone.DayTable = new int[this.DayTable.Length];
				this.DayTable.CopyTo(clone.DayTable, 0);
			}

			if (this.StartTimes != null)
			{
				clone.StartTimes = new ItemsDictionary<StartTime>();
				foreach (var startTime in this.StartTimes)
				{
					clone.StartTimes.Add(startTime.Key, startTime.Value.Clone());
				}
			}

			if (this.StationGroups != null)
			{
				clone.StationGroups = new ItemsDictionary<StationGroup>();
				foreach (var stationGroup in this.StationGroups)
				{
					clone.StationGroups.Add(stationGroup.Key, stationGroup.Value.Clone());
				}
			}

			if (this.Status != null)
				clone.Status = this.Status.Clone();

			return clone;
		}

		public void MergeFromParent(Program parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.DayTable = MergeExtensions.MergeProperty(this.DayTable, parent.DayTable, removeIfMissingFromParent, parentIsMetadata);
				this.SetToRun = MergeExtensions.MergeProperty(this.SetToRun, parent.SetToRun, removeIfMissingFromParent, parentIsMetadata);
				this.Status = MergeExtensions.MergeProperty(this.Status, parent.Status, removeIfMissingFromParent, parentIsMetadata);
               
				this.StartTimes = ItemsDictionary<StartTime>.MergePropertyLists(this.StartTimes, parent.StartTimes, removeIfMissingFromParent, parentIsMetadata);
				this.StationGroups = ItemsDictionary<StationGroup>.MergePropertyLists(this.StationGroups, parent.StationGroups, removeIfMissingFromParent, parentIsMetadata);
			}
		}
  	}
}
