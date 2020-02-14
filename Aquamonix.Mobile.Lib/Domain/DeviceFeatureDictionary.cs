using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class  DeviceFeatureDictionary : ICloneable<DeviceFeatureDictionary>, IMergeable<DeviceFeatureDictionary>
	{
		[DataMember(Name = PropertyNames.ButtonText)]
		public string[] ButtonText { get; set; }

		[DataMember(Name = PropertyNames.ProgressText)]
		public string[] ProgressText { get; set; }

		[DataMember(Name = PropertyNames.PromptText)]
		public string[] PromptText { get; set; }

		[DataMember(Name = PropertyNames.PromptConfirm)]
		public string[] PromptConfirm { get; set; }

		[DataMember(Name = PropertyNames.PromptCancel)]
		public string[] PromptCancel { get; set; }

		public DeviceFeatureDictionary Clone()
		{
			return new DeviceFeatureDictionary()
			{
				ButtonText = this.ButtonText,
				ProgressText = this.ProgressText,
				PromptCancel = this.PromptCancel,
				PromptConfirm = this.PromptConfirm,
				PromptText = this.PromptText
			};
		}

		public void MergeFromParent(DeviceFeatureDictionary parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
			if (parent != null)
			{
				this.ButtonText = MergeExtensions.MergeProperty(this.ButtonText, parent.ButtonText, removeIfMissingFromParent, parentIsMetadata);
				this.ProgressText = MergeExtensions.MergeProperty(this.ProgressText, parent.ProgressText, removeIfMissingFromParent, parentIsMetadata);
				this.PromptText = MergeExtensions.MergeProperty(this.PromptText, parent.PromptText, removeIfMissingFromParent, parentIsMetadata);
				this.PromptConfirm = MergeExtensions.MergeProperty(this.PromptConfirm, parent.PromptConfirm, removeIfMissingFromParent, parentIsMetadata);
				this.PromptCancel = MergeExtensions.MergeProperty(this.PromptCancel, parent.PromptCancel, removeIfMissingFromParent, parentIsMetadata);
			}
		}
	}
}
