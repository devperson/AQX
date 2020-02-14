using System;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Pump : IDomainObjectWithId, ICloneable<Pump>, IMergeable<Pump>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Mode)]
		public string Mode { get; set; }

		[DataMember(Name = PropertyNames.Type)]
		public string Type { get; set; }

		[DataMember(Name = PropertyNames.IsUpdatingStatus)]
		public bool IsUpdatingStatus { get; set; }

		public Pump Clone()
		{
			return new Pump()
			{
				Id = this.Id,
				Name = this.Name,
				Type = this.Type
			};
		}

		public void MergeFromParent(Pump parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
				this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
				this.Type = MergeExtensions.MergeProperty(this.Type, parent.Type, removeIfMissingFromParent, parentIsMetadata);
				this.Mode = MergeExtensions.MergeProperty(this.Mode, parent.Mode, removeIfMissingFromParent, parentIsMetadata); 
			}
		}

		public void ReadyChildIds() { }
	}
}
