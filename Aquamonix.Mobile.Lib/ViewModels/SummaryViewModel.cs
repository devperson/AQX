using System;
using System.Linq;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class SummaryViewModel
	{
		public string FeatureDescriptionText { get; set; }

		public string FeatureByLineText { get; set; }

		public IEnumerable<string> HeaderTextLines { get; set; }

		public SummaryViewModel(Summary summary)
		{
			this.FeatureDescriptionText = summary?.BriefTexts?.Values?.LastOrDefault()?.GetValue(); //?.Values?.LastOrDefault();

			this.FeatureByLineText = summary?.SubTexts?.Values?.LastOrDefault()?.GetValue(); //?.Values?.LastOrDefault();

			var headerTextLines = new List<string>();
			if (summary?.Texts != null)
			{
				foreach (var value in summary.Texts.Values)
				{
					headerTextLines.Add(value.GetValue()); //.Values.Last()); 
				}
			}

			this.HeaderTextLines = headerTextLines;
		}
	}
}
