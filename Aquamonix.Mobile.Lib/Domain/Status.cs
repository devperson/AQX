using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Status : ICloneable<Status>, IMergeable<Status>
	{
		[DataMember(Name = PropertyNames.Values)]
		public ItemsDictionary<StatusValue> Values { get; set; }

		[DataMember(Name = PropertyNames.Value)]
		public string Value { get; set; }

        [DataMember(Name = PropertyNames.Visible)]
        public bool Visible { get; set; }

        public Status Clone()
		{
			var clone = new Status() { Value = this.Value };

			if (this.Values != null)
				clone.Values = this.Values.Clone();

			return clone;
		}

		public void MergeFromParent(Status parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);
				this.Values = ItemsDictionary<StatusValue>.MergePropertyLists(this.Values, parent.Values, removeIfMissingFromParent, parentIsMetadata);
			}
		}
	}
}

