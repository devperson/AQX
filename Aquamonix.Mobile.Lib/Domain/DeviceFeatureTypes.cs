using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	//HARDCODED
	public class DeviceFeatureTypes
	{
		//TODO: should be AlertList? 
		public const string AlertList = "AlarmList";
		public const string CircuitList = "Circuits";
		public const string StationList = "StationList";
		public const string ProgramList = "ProgramList";
		public const string Setting = "Setting";
        public const string PivotFeature = "PivotFeature";
        public const string PivotProgramsFeature = "PivotProgramsList";
        public const string SensorList = "SensorList";
		public const string ScheduleList = "Schedules";
		public const string DeviceStopButton = "DeviceStopButton";
		public const string DeviceNextButton = "DeviceNextButton";
		public const string DevicePrevButton = "DevicePrevButton";
	}
}
