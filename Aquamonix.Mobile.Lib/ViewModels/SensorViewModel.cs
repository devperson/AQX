using System;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Domain
{
	public class SensorViewModel
	{
		public string Id { get; set; }    
		public IEnumerable<SensorValueViewModel> Values { get; private set;}
		public SeverityLevel HighestSeverity { get; set; }

		public SensorViewModel(Sensor sensor)
		{
			this.Id = sensor.Id;

			if (sensor.Values != null)
			{
				var values = new List<SensorValueViewModel>();

				bool isFirst = true;
				foreach (var value in sensor.Values)
				{
					values.Add(new SensorValueViewModel(sensor, value.Value, showName:isFirst));
					isFirst = false;
				}

				this.Values = values;

				//get the highest severity of the values
				foreach (var value in this.Values)
				{
					if (value.Severity > this.HighestSeverity)
						this.HighestSeverity = value.Severity;
				}
			}
		}
	}
}
