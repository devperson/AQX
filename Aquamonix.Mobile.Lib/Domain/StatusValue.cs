using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class StatusValue : ICloneable<StatusValue>, IMergeable<StatusValue>
	{
		[DataMember(Name = PropertyNames.Value)]
		public string Value { get; set; }

		public StatusValue Clone()
		{
			return new StatusValue() { Value = this.Value };
		}

		public void MergeFromParent(StatusValue parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);
			}
		}
	}
}

