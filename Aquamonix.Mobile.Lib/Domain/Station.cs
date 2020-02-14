using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Station : IDomainObjectWithId, ICloneable<Station>, IMergeable<Station>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Zone)]
		public string Zone { get; set; }

		[DataMember(Name = PropertyNames.Type)]
		public string Type { get; set; }

		[DataMember(Name = PropertyNames.Status)]
		public Status Status { get; set; }

		[DataMember(Name = PropertyNames.Flow)]
		public string Flow { get; set; }

		//NOTUSED 
		[DataMember(Name = PropertyNames.Valves)]
		public ItemsDictionary<Valve> Valves { get; set;}

		[DataMember(Name = PropertyNames.IsUpdatingStatus)]
		public bool IsUpdatingStatus { get; set; }

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
		public bool Starting { get; set; }

		[IgnoreDataMember]
		public bool Selected { get; set; }

		public void ReadyChildIds()
		{
			this.Valves?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Valves);
		}

		public void MergeFromParent(Station parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Type = MergeExtensions.MergeProperty(this.Type, parent.Type, removeIfMissingFromParent, parentIsMetadata);
				this.Flow = MergeExtensions.MergeProperty(this.Flow, parent.Flow, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.Zone = MergeExtensions.MergeProperty(this.Zone, parent.Zone, removeIfMissingFromParent, parentIsMetadata);
				this.Status = MergeExtensions.MergeProperty(this.Status, parent.Status, removeIfMissingFromParent, parentIsMetadata);

				this.Valves = ItemsDictionary<Valve>.MergePropertyLists(this.Valves, parent.Valves, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public Station Clone()
		{
			var clone = new Station()
			{
				Flow = this.Flow,
				Id = this.Id,
				Type = this.Type, 
				Zone = this.Zone, 
				Name = this.Name
			};

			if (this.Valves != null)
			{
				this.Valves = new ItemsDictionary<Valve>();
				foreach (var valve in this.Valves)
				{
					clone.Valves.Add(valve.Key, valve.Value.Clone());
				}
			}

			if (this.Status != null)
				clone.Status = this.Status.Clone();

			return clone;
		}
	}
}
