using System;
using System.Linq;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class SensorGroupViewModel
	{
		public string Id { get; set;}
		public string Name { get; set;}

		public IEnumerable<SensorViewModel> Sensors { get; private set; }
		public IEnumerable<SensorValueViewModel> SensorValues { get; private set;}

		public SensorGroupViewModel(SensorGroup sensorGroup)
		{
			this.Id = sensorGroup.Id;
			this.Name = sensorGroup.Name;
			
			if (!String.IsNullOrEmpty(sensorGroup.TimeStamp))
			{
				if (!String.IsNullOrEmpty(sensorGroup.Name))
				{
					this.Name += " " + DateTimeUtil.HowLongAgo(sensorGroup.TimeStamp); 
				}
			}

			List<SensorViewModel> sensors = new List<SensorViewModel>();
			List<SensorValueViewModel> values = new List<SensorValueViewModel>();

			if (sensorGroup.Items != null)
			{
				foreach (var sensor in sensorGroup.Items)
				{
					if (sensor.Value?.Values != null)
					{
						var sensorViewModel = new SensorViewModel(sensor.Value);
						if (sensorViewModel.Values != null && sensorViewModel.Values.Any())
							values.AddRange(sensorViewModel.Values);
					}
				}
			}

			this.Sensors = sensors;
			this.SensorValues = values;
		}
	}
}
