using System;

using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class DeviceBadgeViewModel
	{
		public string Type { get; set; }

		public string Text { get; set; }

		public SeverityLevel SeverityLevel { get; set; }
	}
}
