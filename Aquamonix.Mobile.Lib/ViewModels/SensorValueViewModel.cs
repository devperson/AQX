using System;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Domain
{
	public class SensorValueViewModel
	{
		private SensorValue _sensorValue = null;

		public string Id { get; set;}
		public string SensorName { get; set;}
		public string Name { get; set;}
		public string Value { get; set;}
		public bool HasValue
		{
			get {
				return !String.IsNullOrEmpty(_sensorValue?.Value);
				//return !String.IsNullOrEmpty(this.Value);
			}
		}
		public SeverityLevel Severity { get; set;}

		public SensorValueViewModel(Sensor parent, SensorValue sensorValue, bool showName = false)
		{
			this._sensorValue = sensorValue;
			this.Id = sensorValue.Id;

			if (showName)
			{
				this.Name = sensorValue.Name;
				if (String.IsNullOrEmpty(this.Name))
					this.Name = parent.Name;
			}
			else
				this.Name = String.Empty;

			SeverityLevel severity;
			if (Enum.TryParse<SeverityLevel>(sensorValue.Severity, out severity))
				this.Severity = severity;
			
			this.Value = String.Format("{0} {1} {2}", sensorValue.Value, sensorValue.Units, DateTimeUtil.HowLongAgo(sensorValue.TimeStamp)).Trim(); 
		}
	}
}
