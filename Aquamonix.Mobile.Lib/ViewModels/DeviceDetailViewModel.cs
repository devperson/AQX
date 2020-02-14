using System;
using System.Linq;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Environment;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class DeviceDetailViewModel
	{
		private Action<DeviceDetailViewModel, DeviceFeatureViewModel, DeviceSetting> _changeFeatureSettingValue; 

		public Device Device { get; private set;}      
		public string Id { get; set; }
        public int Number { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Description { get; set; }
		public DateTime? DisabledUntil { get; set; }
		public string FriendlyTypeName { get; private set; }
		public bool Online { get; private set;}
		public bool IsUpdatingStatus { get; private set;}
		public bool SupportsIrrigationNext { get; set; }
		public bool SupportsIrrigationPrev { get; set; }
		public bool SupportsIrrigationStop { get; set; }
		public bool SupportsCircuitsNext { get; set; }
		public bool SupportsCircuitsPrev { get; set; }
		public bool SupportsCircuitsStop { get; set; }
        public string StatusText { get; set;}
        public bool DirectionStatus { get; set; }
        public string DirectionValue { get; set; }
        public bool WetDryStatus { get; set; }
        public string WetDryValue { get; set; }
        public bool SpeedStatus { get; set; }
        public double SpeedValue { get; set; }
        public double AppAmountValue { get; set; }
        public double UnitValue { get; set; }
        public bool UnitStatus { get; set; }
        public bool EndGunStatus { get; set; }
        public string EndGunValue { get; set; }
        public string EndGunTitle { get; set; }
        public bool Aux1Status { get; set; }
        public string Aux1Value { get; set; }
        public string Aux1Title { get; set; }
        public bool Aux2Status { get; set; }
        public string Aux2Value { get; set; }
        public string Aux2Title { get; set; }
        public double ToAngleValue { get; set; }
        public bool AutoReverseValue { get; set; }
        public bool Continuous { get; set; }
        public double Speed { get; set; }
        public double ApplicationAmount { get; set; }
        public DateTime? StatusLastUpdated { get; set;}
		public IEnumerable<Device> DevicesInGroup { get; private set; }
        public bool IsFaultActive { get; private set; }

        
        //Status panel properties
        //=================================================================

        private static string _pivotStatus;
        public string PivotStatus
        {
            get { return _pivotStatus; }
            set { _pivotStatus = value; }
        }

        private static string _programStatus;
        public string ProgramStatus
        {
            get { return _programStatus; }
            set { _programStatus = value; }
        }
        private static bool _autostoprevstatus;
        public bool AutoStopRevStatus
        {
            get { return _autostoprevstatus; }
            set { _autostoprevstatus = value; }
        }
        //=================================================================


        //HARDCODED
        public bool IsGroup
		{
			get { return this.Type == "RGROUP"; }
		}
		//TODO: Can Stopping, Nexting, and Preving be replaced by any server-side property ? 
		public bool Stopping
		{
			get
			{
				return (this?.Device?.Stopping).GetValueOrDefault();
			}
			set
			{
				if (this.Device != null)
					this.Device.Stopping = value;
			}
		}
		public bool Nexting
		{
			get
			{
				return (this?.Device?.Nexting).GetValueOrDefault();
			}
			set
			{
				if (this.Device != null)
					this.Device.Nexting = value;
			}
		}
		public bool Preving
		{
			get
			{
				return (this?.Device?.Preving).GetValueOrDefault();
			}
			set
			{
                    if (this.Device != null)
					this.Device.Preving = value;
			}
		}

		public IEnumerable<SensorGroupViewModel> Sensors { get; set; }
        public IEnumerable<MapViewModel> Map { get; set; }
        public IEnumerable<StationViewModel> Stations { get; set; }
		public IEnumerable<ProgramViewModel> Programs { get; set; }
        public IEnumerable<ProgramViewModel> PivotPrograms { get; set; }
        public IEnumerable<AlertViewModel> Alerts { get; set; }
		public IEnumerable<ScheduleViewModel> Schedules { get; set; }
		public IEnumerable<CircuitViewModel> Circuits { get; set; }
		public IDictionary<string, DeviceBadge> Badges { get; set;}
		public IEnumerable<DeviceFeatureViewModel> Features { get; set; }
		public IEnumerable<PumpViewModel> Pumps { get; set; }

        public DeviceFeatureViewModel StationsFeature
        {
            get
            {
                return this.Features?.Where((r) => r.Id == DeviceFeatureIds.Stations).FirstOrDefault();
            }
        }
        public DeviceFeatureViewModel CircuitsFeature
		{
			get
			{
				return this.Features?.Where((r) => r.Id == DeviceFeatureIds.Circuits).FirstOrDefault();
			}
		}
		public DeviceFeatureViewModel SchedulesFeature
		{
			get
			{
				return this.Features?.Where((r) => r.Id == DeviceFeatureIds.Schedules).FirstOrDefault();
			}
		}
		public DeviceFeatureViewModel ProgramsFeature
		{
			get
			{
				return this.Features?.Where((r) => r.Id == DeviceFeatureIds.Programs).FirstOrDefault();
			}
		}
        public DeviceFeatureViewModel PivotProgramsFeature
        {
            get
            {
                return this.Features?.Where((r) => r.Id == DeviceFeatureIds.PivotPrograms).FirstOrDefault();
            }
        }
        public DeviceFeatureViewModel PivotFeature
        {
            get
            {
                return this.Features?.Where((r) => r.Id == DeviceFeatureIds.PivotFeature).FirstOrDefault();
            }
        }
        public DeviceFeatureViewModel AlertsFeature
		{
			get
			{
				return this.Features?.Where((r) => r.Id == DeviceFeatureIds.Alerts).FirstOrDefault();
			}
		}

		public Action<DeviceDetailViewModel, DeviceFeatureViewModel, DeviceSetting> ChangeFeatureSettingValue 
		{
			get { return this._changeFeatureSettingValue; }
			set { this._changeFeatureSettingValue = WeakReferenceUtility.MakeWeakAction<DeviceDetailViewModel, DeviceFeatureViewModel, DeviceSetting>(value); }
		}

        
        public DeviceDetailViewModel()
		{
		}

		public DeviceDetailViewModel(Device device)
		{
            double value;
			this.Device = device;

			device.MergeFromMetadata();

			this.Id = device.Id;
			this.Name = device.Name;
			this.Type = device.Type;
           if (device.CurrentStep != null)
            { 
                this.DirectionStatus = device.CurrentStep.Direction == null ? false : device.CurrentStep.Direction.Visible;
                this.DirectionValue = device.CurrentStep.Direction == null ? string.Empty : device.CurrentStep.Direction.Value;
                this.WetDryStatus = device.CurrentStep.Wet == null ? false: device.CurrentStep.Wet.Visible;
                this.WetDryValue = device.CurrentStep.Wet == null ? string.Empty : device.CurrentStep.Wet.Value;
                this.SpeedStatus = device.CurrentStep.Speed == null ? true : device.CurrentStep.Speed.Visible;
                double.TryParse(device.CurrentStep?.Speed?.Value, out value);
                this.SpeedValue = value;
                this.Aux1Status = device.CurrentStep.Auxiliary1 == null ? false : device.CurrentStep.Auxiliary1.Visible;
                this.Aux1Value = device.CurrentStep.Auxiliary1 == null ? string.Empty : device.CurrentStep.Auxiliary1.Value; 
                this.Aux2Status = device.CurrentStep.Auxiliary2 == null ? false : device.CurrentStep.Auxiliary2.Visible;
                this.Aux2Value = device.CurrentStep.Auxiliary2 == null ? string.Empty : device.CurrentStep.Auxiliary2.Value;
                this.EndGunStatus = device.CurrentStep.EndGun == null ? false : device.CurrentStep.EndGun.Visible;
                this.EndGunValue = device.CurrentStep.EndGun == null ? string.Empty : device.CurrentStep.EndGun.Value;
                double.TryParse(device.CurrentStep?.ApplicationAmount?.Value, out value);
                this.AppAmountValue = value;
            }
            /*End Gun Title*/
            //this.EndGunTitle



          //  this.Aux1Title = device.MetaData.Device.Features.Items.FirstOrDefault().Value.Program.Steps.Items.FirstOrDefault().Value.Auxiliary1.Dictionary.Title.First().ToString();
          //  this.Aux2Title = device.MetaData.Device.Features.Items.FirstOrDefault().Value.Program.Steps.Items.FirstOrDefault().Value.Auxiliary2.Dictionary.Title.First().ToString();

            this.FriendlyTypeName = DataCache.ApplicationMetadata?.GetFriendlyNameForDeviceType(this.Type);
			this.IsUpdatingStatus = device.IsUpdatingStatus.GetValueOrDefault(); 
            this.IsFaultActive = device.IsFaultActive.GetValueOrDefault();

			if (this.IsGroup)
			{
				this.Description = device.Badges != null && device.Badges.Items.Any() ? device.Badges?.Items?.First().Value?.Texts?.FirstOrDefault() : String.Empty;

				this.DevicesInGroup = device?.Devices?.Values;
				if (this.DevicesInGroup == null)
					this.DevicesInGroup = new List<Device>();
			}
			else
				this.Description = String.Format("{0} {1}", this.FriendlyTypeName, device.Id);
            

            this.SetAlerts(device.Alerts?.Values);
			this.SetStations(device.Stations?.Values);
			this.SetPrograms(device.MetaData.Device.Programs?.Values);
            this.SetPivotPrograms(device.MetaData.Device.Programs?.Values);
            this.SetSensors(device.SensorGroups?.Values);
            this.SetFeatures(device.Features?.Values);
			this.SetCircuits(device.Circuits?.Values);
			this.SetSchedules(device.Schedules?.Values);
			this.SetPumps(device.Pumps?.Values);
            
			if (device.Status != null)
			{
				this.StatusText = device.Status.Value;

				this.StatusLastUpdated = DateTimeUtil.FromUtcDateString(device.Status.UpdatedDateUtc); 
			}

			this.SupportsIrrigationPrev = device.HasFeature(DeviceFeatureIds.IrrigationPrev);
			this.SupportsIrrigationNext = device.HasFeature(DeviceFeatureIds.IrrigationNext);
			this.SupportsIrrigationStop = device.HasFeature(DeviceFeatureIds.IrrigationStop);
			this.SupportsCircuitsPrev = device.HasFeature(DeviceFeatureIds.CircuitsPrev);
			this.SupportsCircuitsNext = device.HasFeature(DeviceFeatureIds.CircuitsNext);
			this.SupportsCircuitsStop = device.HasFeature(DeviceFeatureIds.CircuitsStop);

			this.SetFeatureSummary(DeviceFeatureIds.Stations, device?.Stations?.Summary);
			this.SetFeatureSummary(DeviceFeatureIds.Sensors, device?.SensorGroups?.Summary);
			this.SetFeatureSummary(DeviceFeatureIds.Programs, device?.Programs?.Summary);
            this.SetFeatureSummary(DeviceFeatureIds.PivotPrograms, device?.Programs?.Summary);
            this.SetFeatureSummary(DeviceFeatureIds.Circuits, device?.Circuits?.Summary);
			this.SetFeatureSummary(DeviceFeatureIds.Schedules, device?.Schedules?.Summary);
			this.SetFeatureSummary(DeviceFeatureIds.Pumps, device?.Pumps?.Summary);
		}

        public void SetPivotPrograms(IEnumerable<Program> programs)
        {
            var list = new List<ProgramViewModel>();
            if (programs != null)
            {
                foreach (var program in programs)
                    list.Add(new ProgramViewModel(program));
            }

            this.PivotPrograms = list;
        }

        public void SetAlerts(IEnumerable<Alert> alerts)
		{
			var list = new List<AlertViewModel>();
			if (alerts != null)
			{
				foreach (var alert in alerts)
					list.Add(new AlertViewModel(alert, this.Device)); 
			}

            this.Alerts = list;
		}

        public void SetStations(IEnumerable<Station> stations)
        {
            var list = new List<StationViewModel>();
            if (stations != null)
            {
                foreach (var station in stations)
                    list.Add(new StationViewModel(station));
            }

            this.Stations = list;
        }

        public void SetPrograms(IEnumerable<Program> programs)
		{
			var list = new List<ProgramViewModel>();
			if (programs != null)
			{
				foreach (var program in programs)
					list.Add(new ProgramViewModel(program));
			} 

			this.Programs = list;
		}

		public void SetSensors(IEnumerable<SensorGroup> sensorGroups)
		{
			var list = new List<SensorGroupViewModel>();
			if (sensorGroups != null)
			{
				foreach (var sensorGroup in sensorGroups)
					list.Add(new SensorGroupViewModel(sensorGroup));
			}

			this.Sensors = list;
		}

		public void SetFeatures(IEnumerable<DeviceFeature> features)
		{
			var list = new List<DeviceFeatureViewModel>();
			if (features != null)
			{
				foreach (var feature in features)
				{
					list.Add(DeviceFeatureViewModel.Create(this.Device, feature)); 
				}
			}

			this.Features = list;
		}
		public void SetCircuits(IEnumerable<Circuit> circuits)
		{
			var list = new List<CircuitViewModel>();
			if (circuits != null)
			{
				foreach (var circuit in circuits)
					list.Add(new CircuitViewModel(circuit));
			}

			this.Circuits = list;
		}

		public void SetSchedules(IEnumerable<Schedule> schedules)
		{
			var list = new List<ScheduleViewModel>();
			if (schedules != null)
			{
				foreach (var schedule in schedules)
					list.Add(new ScheduleViewModel(schedule));
			}

			this.Schedules = list;
		}

		public void SetPumps(IEnumerable<Pump> pumps)
		{
			var list = new List<PumpViewModel>();
			if (pumps != null)
			{
				foreach (var pump in pumps)
					list.Add(new PumpViewModel(pump));
			}
			this.Pumps = list;
		}

		private void SetFeatureSummary(string featureId, Summary summary)
		{
			if (summary != null)
			{
				var feature = this.GetFeature(featureId);
				if (feature != null)
					feature.Summary = new SummaryViewModel(summary);
			}
		}

		private DeviceFeatureViewModel GetFeature(string id)
		{
			DeviceFeatureViewModel output = null;
			if (this.Features != null)
				output = this.Features.Where((f) => f.Id == id).FirstOrDefault();

			return output; 
   		}
  	}
}
