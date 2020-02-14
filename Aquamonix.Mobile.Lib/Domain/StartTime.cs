using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class StartTime : IDomainObjectWithId, ICloneable<StartTime>, IMergeable<StartTime>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.StartTime)]
		public string Value { get; set; }

		public StartTime Clone()
		{
			return new StartTime() { Id = this.Id, Value = this.Value };
		}

		public void MergeFromParent(StartTime parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);
			}
		}

		public void ReadyChildIds() { }
	}
}
