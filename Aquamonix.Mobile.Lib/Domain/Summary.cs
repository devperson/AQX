using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Summary : ICloneable<Summary>, IMergeable<Summary>
	{
		[DataMember(Name = PropertyNames.Texts)]
		public ItemsDictionary<SummaryValue> Texts { get; set; }

		[DataMember(Name = PropertyNames.SubTexts)]
		public ItemsDictionary<SummaryValue> SubTexts { get; set; }

		[DataMember(Name = PropertyNames.BriefTexts)]
		public ItemsDictionary<SummaryValue> BriefTexts { get; set; }

		public Summary Clone()
		{
			var clone = new Summary();

			if (this.Texts != null)
				clone.Texts = this.Texts.Clone();

			if (this.SubTexts != null)
				clone.SubTexts = this.SubTexts.Clone();

			if (this.BriefTexts != null)
				clone.BriefTexts = this.BriefTexts.Clone();

			return clone;
		}

		public void MergeFromParent(Summary parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				ItemsDictionary<SummaryValue>.MergePropertyLists(this.Texts, parent.Texts, removeIfMissingFromParent, parentIsMetadata);
				ItemsDictionary<SummaryValue>.MergePropertyLists(this.SubTexts, parent.SubTexts, removeIfMissingFromParent, parentIsMetadata);
				ItemsDictionary<SummaryValue>.MergePropertyLists(this.BriefTexts, parent.BriefTexts, true, parentIsMetadata);
			}
		}
	}

	[DataContract]
	public class SummaryValue : ICloneable<SummaryValue>, IMergeable<SummaryValue>
	{
		[DataMember(Name = PropertyNames.Value)]
		public string Value { get; set; }

		[DataMember(Name=PropertyNames.Values)]
		public string[] Values { get; set;}

		[DataMember(Name = PropertyNames.Severity)]
		public string Severity { get; set;}


		public string GetValue()
		{
			if (!String.IsNullOrEmpty(this.Value))
				return this.Value;

			if (this.Values != null)
				return this.Values.LastOrDefault();

			return String.Empty;
		}

		public SummaryValue Clone()
		{
			return new SummaryValue() { Severity = this.Severity, Values = this.Values, Value = this.Value };
		}

		public void MergeFromParent(SummaryValue parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			this.Value = MergeExtensions.MergeProperty(this.Value, parent.Value, removeIfMissingFromParent, parentIsMetadata);
			this.Values = MergeExtensions.MergeProperty(this.Values, parent.Values, removeIfMissingFromParent, parentIsMetadata);
			this.Severity = MergeExtensions.MergeProperty(this.Severity, parent.Severity, removeIfMissingFromParent, parentIsMetadata);
		}
	}
}

