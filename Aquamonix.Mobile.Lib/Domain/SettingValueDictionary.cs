using System;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class SettingValueDictionary : ICloneable<SettingValueDictionary>
	{
		[DataMember(Name = PropertyNames.ValuePrefix)]
		public string[] ValuePrefix { get; set; }

		[DataMember(Name = PropertyNames.ValueText)]
		public string[] ValueText { get; set; }

		[DataMember(Name = PropertyNames.ButtonText)]
		public string[] ButtonText { get; set; }

		[DataMember(Name = PropertyNames.ProgressText)]
		public string[] ProgressText { get; set; }

		[DataMember(Name = PropertyNames.ProgressPrefix)]
		public string[] ProgressPrefix { get; set; }

		[DataMember(Name = PropertyNames.PromptText)]
		public string[] PromptText { get; set; }

		[DataMember(Name = PropertyNames.PromptPrefix)]
		public string[] PromptPrefix { get; set; }

		[DataMember(Name = PropertyNames.PromptSuffix)]
		public string[] PromptSuffix { get; set; }

		[DataMember(Name = PropertyNames.PromptConfirm)]
		public string[] PromptConfirm { get; set; }

		[DataMember(Name = PropertyNames.PromptCancel)]
		public string[] PromptCancel { get; set; }

		public SettingValueDictionary Clone()
		{
			return new SettingValueDictionary()
			{
				ProgressPrefix = this.ProgressPrefix,
				PromptCancel = this.PromptCancel,
				PromptConfirm = this.PromptConfirm,
				PromptPrefix = this.PromptPrefix,
				PromptSuffix = this.PromptSuffix,
				ValuePrefix = this.ValuePrefix,
				ButtonText = this.ButtonText,
				ProgressText = this.ProgressText,
				ValueText = this.ValueText,
				PromptText = this.PromptText
			};
		}
	}
}
