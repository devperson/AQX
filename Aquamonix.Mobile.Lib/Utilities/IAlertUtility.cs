using System;

using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for showing platform-specific pop-up alerts on the UI, to tell the user something.
    /// </summary>
	public interface IAlertUtility
	{
        /// <summary>
        /// Displays a custom alert.
        /// </summary>
        /// <param name="title">Alert window title</param>
        /// <param name="message">Alert message</param>
        /// <param name="cancelButtonTitle">Text for cancel button</param>
        /// <param name="callback">Action on click</param>
        /// <param name="otherButtons">Text of any other buttons</param>
        void ShowAlert(string title, string message, string cancelButtonTitle = null, Action<int> callback = null, params string[] otherButtons);

        /// <summary>
        /// Displays an alert for an error.
        /// </summary>
        /// <param name="title">Alert window title</param>
        /// <param name="message">Alert message</param>
        /// <param name="callback">Click callback</param>
        void ShowErrorAlert(string title, string message, Action callback = null);

        /// <summary>
        /// Displays an alert for confirmation (confirm/cancel)
        /// </summary>
        /// <param name="title">Alert window title</param>
        /// <param name="message">Alert message</param>
        /// <param name="callback">Click callback</param>
        /// <param name="okButtonText">Text of confirm button</param>
        /// <param name="cancelButtonText">Text of cancel button</param>
		void ShowConfirmationAlert(string title, string message, Action<bool> callback = null, string okButtonText = StringLiterals.OkButtonText, string cancelButtonText = StringLiterals.CancelButtonText);

        /// <summary>
        /// Remove all currently displayed alerts, if any.
        /// </summary>
		void RemoveAllAlerts();
	}
}
