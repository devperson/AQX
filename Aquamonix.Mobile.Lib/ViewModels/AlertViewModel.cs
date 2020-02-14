using System;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class AlertViewModel
	{
        public string Id { get; set;}

		public string DisplayDate { get; set;}

		public string DisplayText { get; set;}

		public AlertSeverity Severity { get; set; }

        public string DeviceId { get; set; }

        public string DeviceName { get; set; }

        public bool Active { get; set;}

		public bool ShowRed { get; set;}

		public DateTime Date { get; set;}

        public DateTime DateUtc { get; set; }

		public AlertViewModel(Alert alert, Device device = null)
		{
			this.Id = alert.Id;
			this.DisplayText = alert.Description;
			this.Active = alert.Active;

			AlertSeverity severity;
			if (Enum.TryParse<AlertSeverity>(alert.Severity, out severity))
				this.Severity = severity;

			DateTime? dateTime = DateTimeUtil.FromUtcDateString(alert.DateTimeUtc);

			if (dateTime != null)
			{
				this.Date = dateTime.Value;
                this.DateUtc = dateTime.Value;
				this.DisplayDate = DateTimeUtil.DateTimeInEnglish(dateTime);  
			}

			if (device != null)
            {
                this.DeviceId = device.Id;
                this.DeviceName = device.Name;
            }

            this.ShowRed = (this.Severity == AlertSeverity.High); 
		}
	}

	public enum AlertSeverity
	{
		Low, 
		Medium,
		High
	}
}
