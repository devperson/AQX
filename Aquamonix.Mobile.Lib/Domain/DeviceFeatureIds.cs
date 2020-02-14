using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Domain
{
	//HARDCODED
	public class DeviceFeatureIds
	{
		public const string IrrigationNext = "IrrigationNext";
		public const string IrrigationPrev = "IrrigationPrev";
		public const string IrrigationStop = "IrrigationStop";
		public const string CircuitsNext = "CircuitsNext";
		public const string CircuitsPrev = "CircuitsPrev";
		public const string CircuitsStop = "CircuitsStop";
		public const string Programs = "ProgramsFeature";
        /* For Pivot*/
        public const string PivotPrograms = "PivotProgramsFeature";
        public const string PivotFeature = "PivotFeature";
        //TODO: should be AlertsFeature? 
        public const string Alerts = "AlarmsFeature";
		public const string Circuits = "Circuits";
		public const string Stations = "StationsFeature";
		public const string Sensors = "SensorsFeature";
		public const string Pumps = "PumpsFeature";
		public const string Schedules = "Schedules";
		public const string ProgramDisable = "ProgramDisable";
		public const string ProgramScaleFactor = "ProgramScaleFactor";
  	}
}
