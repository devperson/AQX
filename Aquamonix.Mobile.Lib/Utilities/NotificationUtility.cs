using System;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for broadcasting application-wide programmatic notifications (platform-specific)
    /// </summary>
	public interface INotificationUtility
	{
		void PostNotification(NotificationType type, Action callback = null); 
	}

    /// <summary>
    /// Static helper class for broadcasting application-wide programmatic notifications (platform-specific)
    /// </summary>
	public static class NotificationUtility
	{
		public static void PostNotification(NotificationType type, Action callback = null)
		{
			if (Environment.Providers.NotificationUtility != null)
				Environment.Providers.NotificationUtility.PostNotification(type, callback); 
		}
	}

	public enum NotificationType
	{
		TabBarHome,
		TabBarAlerts,
		TabBarSettings,
		Reconnected,
		AlertsCountChanged,
        ConnectionStateChanged,
        AuthFailure,
        Activated, 
        ShowReconnecting,
        HideReconnecting, 
        OnAuthSuccess
	}
}
