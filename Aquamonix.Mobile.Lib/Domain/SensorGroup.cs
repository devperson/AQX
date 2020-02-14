using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class SensorGroup : ItemsDictionary<Sensor>, IDomainObjectWithId, ICloneable<SensorGroup>, IMergeable<SensorGroup>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		//[DataMember(Name = "Items")]
		//public ItemsDictionary<Sensor> Items { get; set; }

		[DataMember(Name = PropertyNames.TimeStamp)]
		public string TimeStamp { get; set;}

		public void ReadyChildIds()
		{
			//base.ReadyChildIds();

			this.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Sensors);
		}

		SensorGroup ICloneable<SensorGroup>.Clone()
		{
			var clone = new SensorGroup() { 
				Id = this.Id,
				Name = this.Name 
			};

			var dictClone = base.Clone();
			clone.Items = dictClone.Items;

			return clone;
		}

		public override ItemsDictionary<Sensor> Clone()
		{
			return ((ICloneable<SensorGroup>)this).Clone();
		}

		public void MergeFromParent(SensorGroup parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.TimeStamp = MergeExtensions.MergeProperty(this.TimeStamp, parent.TimeStamp, removeIfMissingFromParent, parentIsMetadata);

				var items  = ItemsDictionary<Sensor>.MergePropertyLists(this, parent, removeIfMissingFromParent, parentIsMetadata);
				this.Items = items.Items;
			}
		}
	}
}
