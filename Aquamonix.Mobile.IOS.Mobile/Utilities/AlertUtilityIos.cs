using System;
using System.Linq;

using UIKit;
using Foundation;

using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.IOS.Utilities
{
    /// <summary>
    /// IOS-specific implementation of IAlertUtility.
    /// </summary>
	public class AlertUtilityIos : IAlertUtility
	{
		private const int MaxAlertChars = 300; 

		private static UIAlertView _currentAlert;

		public void ShowAlert(string title, string message, string cancelButtonTitle = null, Action<int> onClick = null, params string[] otherButtons)
		{
			if (cancelButtonTitle == null)
				cancelButtonTitle = StringLiterals.AlertConfirmButtonText;

			message = AdjustMessage(message);
			LogUtility.LogMessage("Alert: " + message);
			
            //TODO: fix obsolete constructor call 
			var alertView = new UIAlertView(title, message, null, cancelButtonTitle, otherButtons);

			if (onClick != null)
				alertView.Clicked += (o, e) =>
			{
				int buttonIndex = (int)e.ButtonIndex;
				onClick(buttonIndex); 
			};

			_currentAlert = alertView;
			alertView.Show();
		}

		public void ShowErrorAlert(string title, string message, Action callback = null)
		{
			Action<int> onClick = null;

			if (callback != null)
			{
				onClick = (b) => { callback(); };
			}

			ShowAlert(title, message, onClick: onClick);


			//dismiss all keyboards
			UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("resignFirstResponder"), null, null, null); 
		}

		public void ShowConfirmationAlert(string title, string message, Action<bool> callback = null, string okButtonText = StringLiterals.OkButtonText, string cancelButtonText = StringLiterals.CancelButtonText)
		{
			ShowAlert(
				title,
				message,
				cancelButtonText,
				(buttonIndex) =>
				{
					bool confirmed = (buttonIndex == 1);
					if (callback != null)
						callback(confirmed);
				},
				new string[] { okButtonText }
			);
		}

		public void RemoveAllAlerts()
		{
			if (_currentAlert != null)
				_currentAlert.DismissWithClickedButtonIndex(_currentAlert.CancelButtonIndex, false);
		}

		private string AdjustMessage(string message)
		{
			if (message == null)
				message = String.Empty;

			if (message.Length > MaxAlertChars)
				message = message.Substring(0, MaxAlertChars);

			return message;
		}
	}
}
