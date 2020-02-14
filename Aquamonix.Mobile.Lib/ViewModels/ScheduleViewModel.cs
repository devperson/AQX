using System;

using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class ScheduleViewModel
	{
		public string Name { get; set; }
		public bool On { get; set; }
		public string Description { get; set; }
		public string Circuits { get; set; }

		public ScheduleViewModel()
		{
		}

		public ScheduleViewModel(Schedule schedule)
		{
			this.Name = schedule.Name;

			this.On = schedule.SetToRun;

			var subtext2 = schedule.SubTexts.GetIfExists("2")?.Value;
			var subtext3 = schedule.SubTexts.GetIfExists("3")?.Value;
			var subtext4 = schedule.SubTexts.GetIfExists("4")?.Value;

			string description = String.Empty;
			if (!String.IsNullOrEmpty(subtext2))
				description = subtext2;

			if (!String.IsNullOrEmpty(subtext3))
			{
				if (!String.IsNullOrEmpty(description))
					description += " ";
				description += subtext3;
			}

			this.Description = description;
			this.Circuits = subtext4; 
		}
	}
}
