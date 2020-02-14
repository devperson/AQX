using System;

namespace Aquamonix.Mobile.Lib.Environment
{
    /// <summary>
    /// Application settings container.
    /// </summary>
	public static class AppSettings
	{
		public const string LoggingDirectoryNameAppSettingName = "LogsDirectoryName";
		public const string PathSeparatorAppSettingName = "PathSeparator";
		public const string LogFileExtensionAppSettingName = "LogFileExtension";
		public const string LogSeverityAppSettingName = "LogSeverity";
		public const string MaxLogFilesAppSettingName = "MaxLogFiles";
		public const string MaxInfoSeverityLogMessageLengthAppSettingName = "MaxInfoSeverityLogMessageLength"; // not used
		public const string LogEmailRecipientsAppSettingName = "LogEmailRecipients";
		public const string LogEmailRecipientsSeparatorAppSettingName = "LogEmailRecipientsSeparator";  // not used
		public const string DefaultAppServerUriAppSettingName = "DefaultAppServerUri"; 

		public static readonly bool DemoMode = false;
		public static readonly bool UseTimestamps = true;
		public static readonly bool SkipLogin = false;
		public static readonly bool AdvancedView = false;
		public static readonly bool CachingEnabled = true;
		public static readonly bool ProgramStopEnabled = false;
        public static readonly int ReconnectCount = 2;
        public static readonly int AlertsMaxCount = 1000;
        public static readonly string AlertsDismissMinVersion = "0.9.3"; 

        public static TValueType GetAppSettingValue<TValueType>(string appSettingsName, TValueType defaultValue)
		{
			return Providers.AppSettingsUtility.GetAppSettingValue(appSettingsName, defaultValue);
		}
	}
}
