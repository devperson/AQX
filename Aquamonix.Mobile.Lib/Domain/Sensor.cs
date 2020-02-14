using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Sensor : IDomainObjectWithId, ICloneable<Sensor>, IMergeable<Sensor>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Values)]
		public ItemsDictionary<SensorValue> Values { get; set;}

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		public Sensor Clone()
		{
			var clone = new Sensor() { Id = this.Id };

			if (this.Values != null)
			{
				clone.Values = this.Values.Clone();
			}

			return clone;
		}

		public void ReadyChildIds()
		{
			this.Values?.ReadyDictionary();
			//DomainObjectWithId.ReadyChildIdsDictionary(this.Values);
		}

		public void MergeFromParent(Sensor parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.Values = ItemsDictionary<SensorValue>.MergePropertyLists(this.Values, parent.Values, removeIfMissingFromParent, parentIsMetadata);  //MergeExtensions.MergePropertyLists(this.Values, parent.Values, removeIfMissingFromParent, parentIsMetadata);
			}
		}
  	}
}
