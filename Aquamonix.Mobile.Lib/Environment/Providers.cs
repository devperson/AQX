
using System;
using System.Threading.Tasks;

namespace Aquamonix.Mobile.Lib.Environment
{
	/// <summary>
	/// This class will serve as the bucket of services that are exposed from this project.
	/// Any properties that require DependencyService can be resolved in the Application constructor.
	/// </summary>
	public static class Providers
	{
		//the credentials storage service
		//public static IKeychain Keychain;

		//the SQLite context
		//public static ISQLitePlatform SQLitePlatform;

		// File and IO context
		public static Aquamonix.Mobile.Lib.Utilities.IFileUtility FileUtility;

		// WebSockets connectivity
		public static Aquamonix.Mobile.Lib.Utilities.WebSockets.IWebSocketsClient WebSocketsClient;

		// AppSettings
		public static Aquamonix.Mobile.Lib.Utilities.IAppSettingsUtility AppSettingsUtility;

		// Logging
		public static Aquamonix.Mobile.Lib.Utilities.ILogUtility LogUtility;

		// Progress Spinner
		public static Aquamonix.Mobile.Lib.Utilities.IProgressUtility ProgressUtility;

		// Alert popups
		public static Aquamonix.Mobile.Lib.Utilities.IAlertUtility AlertUtility;

		// Json serialization/deserialization
		public static Aquamonix.Mobile.Lib.Utilities.IJsonUtility JsonUtility;

		// Geolocation
		public static Aquamonix.Mobile.Lib.Utilities.ILocationUtility LocationUtility;

		// Notification
		public static Aquamonix.Mobile.Lib.Utilities.INotificationUtility NotificationUtility;
	}
}
