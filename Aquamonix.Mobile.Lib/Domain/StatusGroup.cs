using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class StatusGroup : IDomainObjectWithId, ICloneable<StatusGroup>, IMergeable<StatusGroup>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Statuses)]
		public ItemsDictionary<Status> Statuses { get; set; }

		public StatusGroup Clone()
		{
			var clone = new StatusGroup() { Id = this.Id };

			if (this.Statuses != null)
				clone.Statuses = this.Statuses.Clone();

			return clone;
		}

		public void MergeFromParent(StatusGroup parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Statuses = ItemsDictionary<Status>.MergePropertyLists(this.Statuses, parent.Statuses, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public void ReadyChildIds() { }
	}
}

