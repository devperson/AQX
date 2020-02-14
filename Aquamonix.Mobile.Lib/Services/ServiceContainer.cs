using System;

namespace Aquamonix.Mobile.Lib.Services
{
    /// <summary>
    /// Contains instances of service interface. 
    /// </summary>
	public class ServiceContainer
    {
        public static ServerVersion ServerVersion { get { return ServiceBase.LastServerVersion; } }

        public static IStartPivot PivotService { get; private set; }

        public static IUserService UserService { get; private set; }

        public static IDeviceService DeviceService { get; private set; }

        public static IAlertService AlertService { get; private set; }

        public static ISensorService SensorService { get; private set; }

        public static ISettingsService StatusService { get; private set; }

        public static IStationService StationService { get; private set; }

        public static IProgramService ProgramService { get; private set; }

        public static ICircuitService CircuitService { get; private set; }

        static ServiceContainer()
        {
            if (Environment.AppSettings.DemoMode)
            {
                /*
				UserService = new Mock.UserService();
				StatusService = new Mock.SettingsService();
				AlertService = new Mock.AlertService();
				DeviceService = new Mock.DeviceService();
				SensorService = new Mock.SensorService();
				StationService = new Mock.StationService();
				CircuitService = new Mock.CircuitService();
				ProgramService = new Mock.ProgramService();
				*/
            }
            else
            {
                UserService = new UserService();
                StatusService = new SettingsService();
                AlertService = new AlertService();
                DeviceService = new DeviceService();
                SensorService = new SensorService();
                StationService = new StationService();
                CircuitService = new CircuitService();
                ProgramService = new ProgramService();
                PivotService = new StartPivotDeviceService();

            }
        }

        /// <summary>
        /// Clears/invalidates all caches in all services.
        /// </summary>
		public static void InvalidateCache()
        {
            UserService?.ClearCache();
            StatusService?.ClearCache();
            AlertService?.ClearCache();
            DeviceService?.ClearCache();
            SensorService?.ClearCache();
            StationService?.ClearCache();
            CircuitService?.ClearCache();
            ProgramService?.ClearCache();
            PivotService?.ClearCache();
        }
    }
}
