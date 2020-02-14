using System;

using Foundation;

using Aquamonix.Mobile.IOS.Callbacks;
using Aquamonix.Mobile.Lib.Utilities ;

namespace Aquamonix.Mobile.IOS.Utilities
{
    /// <summary>
    /// IOS-specific implementation of INotificationUtility.
    /// </summary>
	public class NotificationUtilityIos : INotificationUtility
	{
		public void PostNotification(NotificationType type, Action callback = null)
		{
			NSCallback nsCallback = new NSCallback(() =>
			{
				if (callback != null)
					callback();
			});

			NSNotificationCenter.DefaultCenter.PostNotificationName(
				type.ToString(), nsCallback);
		}
	}
}
