using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

using Aquamonix.Mobile.Lib.Extensions;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Environment;

namespace Aquamonix.Mobile.Lib.Domain
{
	[DataContract]
	public class Device : IDomainObjectWithId, ICloneable<Device>, IMergeable<Device>
	{
		[DataMember(Name = PropertyNames.Id)]
		public string Id { get; set;}

		[IgnoreDataMember]
		public string FriendlyTypeName { get; set; }

		[DataMember(Name = PropertyNames.Name)]
		public string Name { get; set; }

		[DataMember(Name = PropertyNames.Number)]
		public string Number { get; set; }

		[DataMember(Name = PropertyNames.Status)]
		public DeviceStatus Status { get; set; }

		[DataMember(Name = PropertyNames.Type)]
		public string Type
		{
			get; set;
		} 

		//NOTUSED
		//[DataMember(Name = "BandWidth")]
		//public string BandWidth { get; set; }

		[DataMember(Name = PropertyNames.Location)]
		public DeviceLocation Location { get; set; }

        [DataMember(Name = PropertyNames.AlertsCount)]
		public int? AlertsCount { get; set; }

		[DataMember(Name = PropertyNames.Badges)]
		public ItemsDictionary<DeviceBadge> Badges { get; set;}

		[DataMember(Name = PropertyNames.MonthOverBudgetPercent)]
		public double? MonthOverBudgetPercent { get; set;}

		[DataMember(Name = PropertyNames.Programs)]
		public ItemsDictionary<Program> Programs { get; set; }

		[DataMember(Name = PropertyNames.Stations)]
		public ItemsDictionary<Station> Stations { get; set; }

		[DataMember(Name = PropertyNames.Pumps)]
		public ItemsDictionary<Pump> Pumps { get; set; }

		[DataMember(Name = PropertyNames.Settings)]
		public ItemsDictionary<DeviceSetting> Settings { get; set; }

		[DataMember(Name = PropertyNames.Features)]
		public ItemsDictionary<DeviceFeature> Features { get; set; }

		[DataMember(Name = PropertyNames.Alerts)]
		public ItemsDictionary<Alert> Alerts { get; set; }

		/*
		[IgnoreDataMember]
		public Dictionary<string, string> RemoveFeatures { get; set;}
		*/

		[DataMember(Name = PropertyNames.MetaData)]
		public DeviceMetadata MetaData { get; set; }

		[DataMember(Name = PropertyNames.SensorGroups)]
		public ItemsDictionary<SensorGroup> SensorGroups { get; set; }

		//NOTUSED
		[DataMember(Name = PropertyNames.StatusGroups)]
		public ItemsDictionary<StatusGroup> StatusGroups { get; set; }

		[DataMember(Name = PropertyNames.Circuits)]
		public ItemsDictionary<Circuit> Circuits { get; set; }

		[DataMember(Name = PropertyNames.Schedules)]
		public ItemsDictionary<Schedule> Schedules { get; set; }

		[DataMember(Name = PropertyNames.Devices)]
		public ItemsDictionary<Device> Devices { get; set;}

		[DataMember(Name = PropertyNames.IsUpdatingStatus)]
		public bool? IsUpdatingStatus { get; set; }

		[IgnoreDataMember]
		public bool Stopping { get; set;}

		[IgnoreDataMember]
		public bool Nexting { get; set;}

		[IgnoreDataMember]
		public bool Preving { get; set; }

		[IgnoreDataMember]
		public int? ActiveAlertsCount { get; set; }

        #region For Pivot

        [DataMember(Name = PropertyNames.LengthMetres)]
        public DeviceLengthValue LengthMetres { get; set; }

        [DataMember(Name = PropertyNames.AngleOffset)]
        public AngleOffsetValue AngleOffset { get; set; }

        [DataMember(Name = PropertyNames.AngleLimits)]
        public AngleLimitsValue AngleLimits { get; set; }

        [DataMember(Name = PropertyNames.CurrentAngle)]
        public ValueVisible CurrentAngle { get; set; }

        [DataMember(Name = PropertyNames.CurrentStep)]
        public CurrentStepValue CurrentStep  { get; set; }

        [DataMember(Name = PropertyNames.SubTexts)]
        public ItemsDictionary<DeviceSubTexts> SubTexts { get; set; }

        [DataMember(Name = PropertyNames.ClockwiseDirection)]
        public bool ClockwiseDirection { get; set; }

        [DataMember(Name = PropertyNames.IsFaultActive)]
        public bool? IsFaultActive { get; set; }
        #endregion

        public void ReadyChildIds()
		{
			if (this.MetaData != null)
				this.MetaData.ReadyChildIds();

			this.Stations?.ReadyDictionary();
			this.Pumps?.ReadyDictionary();
			this.Programs?.ReadyDictionary();
			this.Features?.ReadyDictionary();
			this.Settings?.ReadyDictionary();
			this.SensorGroups?.ReadyDictionary();
			this.StatusGroups?.ReadyDictionary();
			this.Alerts?.ReadyDictionary();
			this.Circuits?.ReadyDictionary();
			this.Schedules?.ReadyDictionary();
			this.Devices?.ReadyDictionary();
		}

		public virtual void MergeFromApplicationMetadata(ApplicationMetadata metadata, string deviceType)
		{
			ExceptionUtility.Try(() =>
			{
				LogUtility.LogMessage("Merging device " + this.Id + " from application metadata.");

				if (metadata != null)
				{
					if (this.Badges != null)
					{
						if (metadata.Badges != null && metadata.Badges.Count > 0)
						{
							foreach (var badge in this.Badges)
							{
								if (metadata.Badges.ContainsKey(badge.Key))
									badge.Value.MergeFromParent(metadata.Badges[badge.Key], false, true);
							}
						}
					}

					if (metadata.DeviceTypes != null)
					{
						if (deviceType != null && metadata.DeviceTypes.ContainsKey(deviceType))
						{
							var deviceTypeSpec = metadata.DeviceTypes[deviceType];

							//Name 
							this.FriendlyTypeName = deviceTypeSpec.Name;

							//Features
							this.Features = ItemsDictionary<DeviceFeature>.MergePropertyLists(this.Features, deviceTypeSpec.Features, false, true);

							//Settings 
							this.Settings = ItemsDictionary<DeviceSetting>.MergePropertyLists(this.Settings, deviceTypeSpec.Settings, false, true);

							if (this.Features != null)
							{
								foreach (var feature in this.Features)
								{

                                    try
                                    {
                                        if (feature.Value.Type.ToLower() == "pivotfeature")
                                        {
                                            feature.Value.Unit = deviceTypeSpec.Features.Items.Values.Where(x => x.Type.ToLower() == "pivotfeature").FirstOrDefault().Program.Steps["-1"].Speed.Units;
                                        }
                                    }
                                    catch(Exception)
                                    {

                                    }
                                    if (!String.IsNullOrEmpty(feature.Value.SettingName))
									{
										if (this.Settings.ContainsKey(feature.Value.SettingName))
											feature.Value.Setting = this.Settings[feature.Value.SettingName];
									}
								}
							}
						}
					}
				}
			});
		}

		public void MergeFromMetadata()
		{
			this.MergeFromApplicationMetadata(DataCache.ApplicationMetadata, this.Type);

			if (this.MetaData != null && this.MetaData.Device != null)
			{
				this.MergeFromParent(this.MetaData.Device, false, true);
			}
		}

		public void MergeAlert(Alert alert)
		{
			ExceptionUtility.Try(() =>
			{
				bool foundExisting = false;
				if (this.Alerts != null)
				{
					foreach (var deviceAlert in this.Alerts)
					{
						if (deviceAlert.Value.Id == alert.Id)
						{
							foundExisting = true;
							deviceAlert.Value.UpdateFrom(alert);
							break;
						}
					}
				}

                if (!foundExisting)
                {
                    if (this.Alerts == null)
                        this.Alerts = new ItemsDictionary<Alert>(); 
                    this.Alerts.Add(alert.Id, alert);
                }

				this.UpdateAlertCounts();
			}); 
		}

		public void MergeFromParent(Device parent, bool removeIfMissingFromParent, bool parentIsMetadata)
		{
            ExceptionUtility.Try((Action)(() =>
			{
				LogUtility.LogMessage("Merging device " + this.Id + " from parent.");

				if (parent != null)
				{
					this.Id = MergeExtensions.MergeProperty(this.Id, parent.Id, removeIfMissingFromParent, parentIsMetadata);
					this.Type = MergeExtensions.MergeProperty(this.Type, parent.Type, removeIfMissingFromParent, parentIsMetadata);
					this.Name = MergeExtensions.MergeProperty(this.Name, parent.Name, removeIfMissingFromParent, parentIsMetadata);
					this.FriendlyTypeName = MergeExtensions.MergeProperty(this.FriendlyTypeName, parent.FriendlyTypeName, removeIfMissingFromParent, parentIsMetadata);
					//this.BandWidth = MergeExtensions.MergeProperty(this.BandWidth, parent.BandWidth, removeIfMissingFromParent, parentIsMetadata);
					this.AlertsCount = MergeExtensions.MergeProperty(this.AlertsCount, parent.AlertsCount, removeIfMissingFromParent, parentIsMetadata);
					this.Number = MergeExtensions.MergeProperty(this.Number, parent.Number, removeIfMissingFromParent, parentIsMetadata);
					this.MonthOverBudgetPercent = MergeExtensions.MergeProperty(this.MonthOverBudgetPercent, parent.MonthOverBudgetPercent, removeIfMissingFromParent, parentIsMetadata);
					this.IsUpdatingStatus = MergeExtensions.MergeProperty(this.IsUpdatingStatus, parent.IsUpdatingStatus, removeIfMissingFromParent, parentIsMetadata);
					this.ActiveAlertsCount = MergeExtensions.MergeProperty(this.ActiveAlertsCount, parent.ActiveAlertsCount, removeIfMissingFromParent, parentIsMetadata);
                    this.IsFaultActive = MergeExtensions.MergeProperty(this.IsFaultActive, parent.IsFaultActive, removeIfMissingFromParent, parentIsMetadata);

                    this.CurrentStep = MergeExtensions.MergeProperty(this.CurrentStep, parent.CurrentStep, removeIfMissingFromParent, parentIsMetadata);
                    this.CurrentAngle = MergeExtensions.MergeProperty(this.CurrentAngle, parent.CurrentAngle, removeIfMissingFromParent, parentIsMetadata);
                    this.AngleOffset = MergeExtensions.MergeProperty(this.AngleOffset, parent.AngleOffset, removeIfMissingFromParent, parentIsMetadata);
                    this.Programs = MergeExtensions.MergeProperty(this.Programs, parent.Programs, removeIfMissingFromParent, parentIsMetadata);
                    this.Features = MergeExtensions.MergeProperty(this.Features, parent.Features, removeIfMissingFromParent, parentIsMetadata);
                    this.Devices = MergeExtensions.MergeProperty(this.Devices, parent.Devices, removeIfMissingFromParent, parentIsMetadata);

                    //status
                    if (this.Status == null)
						this.Status = parent.Status;

					if (this.Status != null)
						this.Status.MergeFromParent(parent.Status, removeIfMissingFromParent, parentIsMetadata);

					/*
					//pumps summary
					if (this.PumpsSummary == null)
						this.PumpsSummary = parent.PumpsSummary;

					if (this.PumpsSummary != null)
						this.PumpsSummary.MergeFromParent(parent.PumpsSummary, removeIfMissingFromParent, parentIsMetadata);

					//stations summary
					if (this.StationsSummary == null)
						this.StationsSummary = parent.StationsSummary;

					if (this.StationsSummary != null)
						this.StationsSummary.MergeFromParent(parent.StationsSummary, removeIfMissingFromParent, parentIsMetadata);

					//programs summary
					if (this.ProgramsSummary == null)
						this.ProgramsSummary = parent.ProgramsSummary;

					if (this.ProgramsSummary != null)
						this.ProgramsSummary.MergeFromParent(parent.ProgramsSummary, removeIfMissingFromParent, parentIsMetadata);

					//sensors summary
					if (this.SensorsSummary == null)
						this.SensorsSummary = parent.SensorsSummary;

					if (this.SensorsSummary != null)
						this.SensorsSummary.MergeFromParent(parent.SensorsSummary, removeIfMissingFromParent, parentIsMetadata);
					*/

					//Badges 
					this.Badges = ItemsDictionary<DeviceBadge>.MergePropertyLists(this.Badges, parent.Badges, removeIfMissingFromParent, parentIsMetadata);

					//Stations
					this.Stations = ItemsDictionary<Station>.MergePropertyLists(this.Stations, parent.Stations, removeIfMissingFromParent, parentIsMetadata);

					//Programs 
					this.Programs = ItemsDictionary<Program>.MergePropertyLists(this.Programs, parent.Programs, removeIfMissingFromParent, parentIsMetadata);

					//Features
					this.Features = ItemsDictionary<DeviceFeature>.MergePropertyLists(this.Features, parent.Features, removeIfMissingFromParent, parentIsMetadata);

					//Settings 
					this.Settings = ItemsDictionary<DeviceSetting>.MergePropertyLists(this.Settings, parent.Settings, removeIfMissingFromParent, parentIsMetadata);

					//SensorGroups 
					this.SensorGroups = ItemsDictionary<SensorGroup>.MergePropertyLists(this.SensorGroups, parent.SensorGroups, removeIfMissingFromParent, parentIsMetadata);

					//StatusGroups 
					this.StatusGroups = ItemsDictionary<StatusGroup>.MergePropertyLists(this.StatusGroups, parent.StatusGroups, removeIfMissingFromParent, parentIsMetadata);

					//StatusGroups 
					this.Alerts = ItemsDictionary<Alert>.MergePropertyLists(this.Alerts, parent.Alerts, removeIfMissingFromParent, parentIsMetadata);

					//Circuits 
					this.Circuits = ItemsDictionary<Circuit>.MergePropertyLists(this.Circuits, parent.Circuits, removeIfMissingFromParent, parentIsMetadata);

					//Schedules 
					this.Schedules = ItemsDictionary<Schedule>.MergePropertyLists(this.Schedules, parent.Schedules, removeIfMissingFromParent, parentIsMetadata);

					//Pumps
					this.Pumps = ItemsDictionary<Pump>.MergePropertyLists(this.Pumps, parent.Pumps, removeIfMissingFromParent, parentIsMetadata);

					//Devices
					this.Devices = ItemsDictionary<Device>.MergePropertyLists(this.Devices, parent.Devices, removeIfMissingFromParent, parentIsMetadata);

					//Metadata
					if (this.MetaData == null)
						this.MetaData = parent.MetaData;

					this.ProcessRemoveLists(); 
				}
			}));
   		}

		public Device Clone()
		{
			var clone = new Device()
			{
				Id = this.Id,
				Name = this.Name,
				Type = this.Type,
				FriendlyTypeName = this.FriendlyTypeName,
				Number = this.Number,
				AlertsCount = this.AlertsCount,
				//BandWidth = this.BandWidth,
				Location = this.Location,               
				IsUpdatingStatus = this.IsUpdatingStatus, 
				ActiveAlertsCount = this.ActiveAlertsCount,
                /* For Pivot */
                LengthMetres = this.LengthMetres,
                AngleOffset = this.AngleOffset,
                CurrentStep = this.CurrentStep,
                CurrentAngle = this.CurrentAngle,
                IsFaultActive = this.IsFaultActive,
                Programs = this.Programs,
                Features = this.Features,
                Devices = this.Devices
            };

			if (this.Alerts != null)
				clone.Alerts = this.Alerts.Clone();

			if (this.Badges != null)
				clone.Badges = this.Badges.Clone();

			if (this.Circuits != null)
				clone.Circuits = this.Circuits.Clone();

			if (this.Devices != null)
				clone.Devices = this.Devices.Clone();

			if (this.Features != null)
				clone.Features = this.Features.Clone();

			if (this.Pumps != null)
				clone.Pumps = this.Pumps.Clone();

			if (this.MetaData != null)
				clone.MetaData = this.MetaData.Clone(); 

			return clone;
		}

		public bool HasFeature(string featureName)
		{
			bool output = false;

			if (this.Features != null)
			{
				output = (this.Features.ContainsKey(featureName));
			}

			return output;
		}

		public void ProcessRemoveLists()
		{
			if (this.Features != null)
				this.Features.ProcessRemoveList();

			if (this.Settings != null)
				this.Settings.ProcessRemoveList();

			if (this.Devices != null)
				this.Devices.ProcessRemoveList();

			if (this.Badges != null)
				this.Badges.ProcessRemoveList();

			if (this.Programs != null)
				this.Programs.ProcessRemoveList();

			if (this.Stations != null)
				this.Stations.ProcessRemoveList();

			if (this.Pumps != null)
				this.Pumps.ProcessRemoveList();

			if (this.Alerts != null)
				this.Alerts.ProcessRemoveList();

			if (this.SensorGroups != null)
				this.SensorGroups.ProcessRemoveList();

			if (this.StatusGroups != null)
				this.StatusGroups.ProcessRemoveList();

			if (this.Schedules != null)
				this.Schedules.ProcessRemoveList();

			if (this.Circuits != null)
				this.Circuits.ProcessRemoveList();
		}

		public void UpdateAlertCounts()
		{
			int alerts = this.Alerts.Count;
			int active = 0; 

			foreach (var alert in this.Alerts)
			{
				if (alert.Value.Active)
					active++;
			}

			this.AlertsCount = alerts;
			this.ActiveAlertsCount = active;
		}
  	}
}