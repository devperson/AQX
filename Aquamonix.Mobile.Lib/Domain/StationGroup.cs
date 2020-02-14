using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{       
	[DataContract]
	public class StationGroup : IDomainObjectWithId, ICloneable<StationGroup>, IMergeable<StationGroup>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.RunTimeMinutes)]
		public int? RunTimeMinutes { get; set; }

		[DataMember(Name = PropertyNames.Stations)]
		public ItemsDictionary<Station> Stations { get; set; }


		public void ReadyChildIds()
		{
			this.Stations?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Stations);
		}

		public StationGroup Clone()
		{
			var clone = new StationGroup()
			{
				Id = this.Id,
				Name = this.Name,
				RunTimeMinutes = this.RunTimeMinutes
			};

			if (this.Stations != null)
			{
				clone.Stations = new ItemsDictionary<Station>();
				foreach (var station in this.Stations)
					clone.Stations.Add(station.Key, station.Value.Clone());
			}

			return clone;
		}

		public void MergeFromParent(StationGroup parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.RunTimeMinutes = MergeExtensions.MergeProperty(this.RunTimeMinutes, parent.RunTimeMinutes, removeIfMissingFromParent, parentIsMetadata);

				this.Stations = ItemsDictionary<Station>.MergePropertyLists(this.Stations, parent.Stations, removeIfMissingFromParent, parentIsMetadata);
			}
		}
	}
}
