using System;
using System.Linq;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.ViewModels
{
	public class DeviceFeatureViewModel
	{
		private string _promptValue; 

		public string Id { get; set; }
		public string Type { get; set; }
		public string DisplayText { get; set; }
		public string Destination { get; set; }
		public virtual string Description 
		{
			get
			{
				return this.Summary?.FeatureDescriptionText;
			}
		}
		public virtual string DescriptionByLine
		{
			get
			{
				return this.Summary?.FeatureByLineText;
			}
		}
		public string PromptValue
		{
			get {
				if (this._promptValue == null)
					return this.Value;

				return this._promptValue;
			}
			set {
				this._promptValue = value;
			}
		}
		public string Value { get; set; }
		public string ButtonText { get; set; }
		public string ProgressText { get; set; }
		public string PromptText { get; set; }
		public string PromptConfirm { get; set; }
		public string PromptCancel { get; set; }
		public string Variation { get; set;}
		public virtual bool ShowRed { get { return false; } }
		public bool ValueChanging { get; set; }
		public bool Updatable { get; set; }
		public bool Editable { get; set; }
		public DeviceFeatureRowDisplayType DisplayType { get; set; }
		public DeviceFeature Feature { get; private set;}
		public SettingValueDictionary SettingsValueDictionary { get; set;}
		public SummaryViewModel Summary { get; set;}
		public IEnumerable<string> HeaderSummaryTexts
		{
			get
			{
				return this.Summary?.HeaderTextLines; 
			}
		}
		protected Device Device { get; set;}

		public DeviceFeatureViewModel()
		{
		}

		public DeviceFeatureViewModel(Device device, DeviceFeature feature)
		{
			this.Device = device;
			this.Feature = feature;

			this.Id = feature.Id;
			this.Type = feature.Type;
			this.DisplayType = DeviceFeatureRowDisplayType.None;
			this.Updatable = feature.Updatable.GetValueOrDefault();
			this.Variation = feature.Variation;

			if (feature.Setting != null) {
				this.Editable = feature.Setting.Editable.GetValueOrDefault();
			}

			if (feature.Dictionary != null)
			{
				this.ButtonText = feature.Dictionary.ButtonText?.LongestString();
				this.ProgressText = feature.Dictionary.ProgressText?.LastOrDefault();
				this.PromptText = feature.Dictionary.PromptText?.LastOrDefault();
				this.PromptConfirm = feature.Dictionary.PromptConfirm?.LastOrDefault();
				this.PromptCancel = feature.Dictionary.PromptCancel?.LastOrDefault();
			}

			if (feature.Setting != null)
			{
				this.SettingsValueDictionary = this.Feature?.Setting?.Values?.Values?.FirstOrDefault()?.Dictionary;
			}

			switch (feature.Type)
			{
				case DeviceFeatureTypes.AlertList:
					this.DisplayText = StringLiterals.Alerts;
					this.Destination = StringLiterals.Alerts;
					this.DisplayType = DeviceFeatureRowDisplayType.Normal;
					break;

				case DeviceFeatureTypes.StationList:
                    this.DisplayText = StringLiterals.Stations;
                    this.Destination = StringLiterals.Stations;
					this.DisplayType = DeviceFeatureRowDisplayType.Normal;
					break;

				case DeviceFeatureTypes.ProgramList:
					this.DisplayText = StringLiterals.Programs;
					this.Destination = StringLiterals.Programs;
					this.DisplayType = DeviceFeatureRowDisplayType.Normal;
					break;

				case DeviceFeatureTypes.CircuitList:
					this.DisplayText = StringLiterals.Circuits;
					this.Destination = StringLiterals.Circuits;
					this.DisplayType = DeviceFeatureRowDisplayType.Normal;
					break;

				case DeviceFeatureTypes.ScheduleList:
					this.DisplayText = StringLiterals.Schedules;
					this.Destination = StringLiterals.Schedules;
					this.DisplayType = DeviceFeatureRowDisplayType.Normal;
					break;

				case DeviceFeatureTypes.Setting:
					
					if (feature.Setting != null)
					{
						var settingValue = feature?.Setting?.Values?.Values?.FirstOrDefault();
						if (settingValue.DisplayType == DeviceSettingDisplayTypes.Slider)
						{
							this.DisplayText = String.Empty; //String.Format("{0} {1}{2}", settingValue?.Dictionary?.ValuePrefix?.LastOrDefault(), settingValue?.Value, settingValue?.Units);
							this.Destination = String.Empty;
							this.DisplayType = DeviceFeatureRowDisplayType.Slider;
						}
						else if (settingValue.DisplayType == DeviceSettingDisplayTypes.Radio)
						{
							this.DisplayText = feature?.Setting?.Id;
							this.DisplayType = DeviceFeatureRowDisplayType.Special;

							if (this.Editable)
								this.Destination = feature?.Setting?.Id;
							else
								this.Destination = String.Empty;
						}
					}

					break;

				case DeviceFeatureTypes.SensorList:
					this.DisplayText = String.Empty;
					this.Destination = String.Empty;
					this.DisplayType = DeviceFeatureRowDisplayType.Sensors;
					break;

                case DeviceFeatureTypes.PivotFeature:
                    this.DisplayText = String.Empty;
                    this.Destination = StringLiterals.Pivot;
                    this.DisplayType = DeviceFeatureRowDisplayType.PivotFeature;
                    break;
                case DeviceFeatureTypes.PivotProgramsFeature:
                    this.DisplayText = "Programs";
                    this.Destination = "PivotPrograms";
                    this.DisplayType = DeviceFeatureRowDisplayType.Normal;
                    break;
               
            }
		}


		public bool IsVisible(Device parentDevice)
		{
			if (this.DisplayType == DeviceFeatureRowDisplayType.Sensors)
			{
				return this.CountVisibleSensors(parentDevice?.SensorGroups?.Values) > 0;
			}

			if (this.DisplayType == DeviceFeatureRowDisplayType.None)
				return false;

			return true;
		}

		private int CountVisibleSensors(IEnumerable<SensorGroup> groups)
		{
			int output = 0;

			if (groups != null)
			{
				List<SensorGroupViewModel> groupViewModels = new List<SensorGroupViewModel>();
				foreach (var group in groups)
					groupViewModels.Add(new SensorGroupViewModel(group));

				ExceptionUtility.Try(() =>
				{
					foreach (var group in groupViewModels)
					{
						if (group.SensorValues != null && group.SensorValues.Count() > 0)
						{
							if (!String.IsNullOrEmpty(group.Name))
							{
								int childValues = 0;

								foreach (var value in group.SensorValues)
								{
									if (value.HasValue) // && !String.IsNullOrEmpty(value.Name))
								{
										childValues++;
									}
								}

								if (childValues > 0)
								{
									output += childValues;
								}
							}
						}
					}
				});
			}

			return output;
		}

		public static DeviceFeatureViewModel Create(Device device, DeviceFeature feature)
		{
			if (feature.Type == DeviceFeatureTypes.Setting && feature?.Setting?.Id == "ProgramDisable")
				return (new ProgramDisableFeatureViewModel(device, feature));

			if (feature.Type == DeviceFeatureTypes.AlertList)
				return new AlertFeatureViewModel(device, feature);

			return (new DeviceFeatureViewModel(device, feature));
		}
	}

	public class AlertFeatureViewModel : DeviceFeatureViewModel
	{
		public override string Description
		{
			get
			{
				if (this.Device.ActiveAlertsCount.GetValueOrDefault() > 0)
					return String.Format("{0} active", this.Device.ActiveAlertsCount.GetValueOrDefault());

				return base.Description;
			}
		}

		public override bool ShowRed
		{
			get
			{
				return this.Device?.ActiveAlertsCount.GetValueOrDefault() > 0;
			}
		}

		public AlertFeatureViewModel(Device device, DeviceFeature feature) : base(device, feature)
		{
		}
	}


	//HARDCODED (lot of stuff regarding ProgramDisable) 
	public class ProgramDisableFeatureViewModel : DeviceFeatureViewModel
	{
		public SettingValueDictionary EnableDictionary { get; private set;}

		public SettingValueDictionary DisableDictionary { get; private set; }

		public SettingValueDictionary DisableTimerDictionary { get; private set; }

		//public int DurationInMinutes { get; set;}

		public DateTime? EndDateTimeUtc { get; set;}

		public string EnableValue { get; private set;}

		public string DisableValue { get; private set; }

		public string DisableTimerValue { get; private set;}

		public string SettingName { get; private set;}

		public bool IsEnabled
		{
			get
			{
				return !String.IsNullOrEmpty(this.Value) && this.Value == this.EnableValue;
			}
		}

		public bool IsDisabledOnTimer
		{
			get
			{
				return !String.IsNullOrEmpty(this.Value) && this.Value == this.DisableTimerValue;
			}
		}

		public bool IsDisabledIndefinitely
		{
			get
			{
				return !String.IsNullOrEmpty(this.Value) && this.Value == this.DisableValue;
			}
		}

		public ProgramDisableFeatureViewModel(Device device, DeviceFeature feature) : base(device, feature)
		{
			//HARDCODED
			var disableSettingValue = feature?.Setting?.Values?.Items.Where((v) => v.Key == "ProgramDisable")?.FirstOrDefault().Value;
			//var durationSettingValue = feature?.Setting?.Values.Where((v) => v.Key == "DurationInMinutes")?.FirstOrDefault().Value;
			var durationSettingValue = feature?.Setting?.Values?.Items.Where((v) => v.Key == "EndDateTimeUtc")?.FirstOrDefault().Value;

			//ProgramDisable setting 
			if (disableSettingValue != null)
			{
				var valueEnum = disableSettingValue?.Enum;
				var currentValue = disableSettingValue?.Value;

				if (currentValue != null && valueEnum != null)
				{
					var currentValueEnum = valueEnum?.Items.Where((i) => i.Key == currentValue)?.FirstOrDefault().Value;
					if (currentValueEnum != null)
					{
						this.DisplayText = currentValueEnum?.Dictionary?.ValueText?.FirstOrDefault();
					}
				}
			}

			//Duration setting 
			if (durationSettingValue != null)
			{
				var currentValue = durationSettingValue?.Value;
				//int duration;
				//if (Int32.TryParse(currentValue, out duration))
				if (currentValue != null)
					this.EndDateTimeUtc = DateTimeUtil.FromUtcDateString(currentValue);
			}

			if (feature.Setting != null)
			{
				this.SettingName = feature.Setting.Name;
			}

			if (String.IsNullOrEmpty(this.DisplayText))
				this.DisplayText = this.SettingName;

			//current value 
			this.Value = disableSettingValue?.Value;

			//dictionaries
			//HARDCODED
			var setting = this.Feature.Setting;
			var enumValue = setting?.Values?.Values.Where((v) => v.Type == "EnumValues").FirstOrDefault();

			//HARDCODED
			var enableSetting = enumValue?.Enum?.Values?.Where((e) => e.Name == "Enable").FirstOrDefault();
			var disableIndefSetting = enumValue?.Enum?.Values?.Where((e) => e.Name == "Disable indefinitely").FirstOrDefault();
			var disableTimerSetting = enumValue?.Enum?.Values?.Where((e) => e.Name == "Disable on timer").FirstOrDefault();

			this.EnableDictionary = enableSetting?.Dictionary;
			this.DisableDictionary = disableIndefSetting?.Dictionary;
			this.DisableTimerDictionary = disableTimerSetting?.Dictionary;

			//values
			//HARDCODED
			this.EnableValue = disableSettingValue.Enum.Items.Where((i) => i.Value.Name == "Enable")?.FirstOrDefault().Key;
			this.DisableValue = disableSettingValue.Enum.Items.Where((i) => i.Value.Name == "Disable indefinitely")?.FirstOrDefault().Key;
			this.DisableTimerValue = disableSettingValue.Enum.Items.Where((i) => i.Value.Name == "Disable on timer")?.FirstOrDefault().Key;

			//if (String.IsNullOrEmpty(this.DisplayText))
			//	this.DisplayText = "Enable/Disable"; 
		}
	}
}
