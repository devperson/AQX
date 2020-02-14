using System;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Environment;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Static helper class for showing UI alerts to users.
    /// </summary>
	public static class AlertUtility
    {
        public static void ShowAlert(string title, string message, string cancelButtonTitle = null, Action<int> onClick = null, params string[] otherButtons)
        {
            if (Providers.AlertUtility != null)
                Providers.AlertUtility.ShowAlert(title, message, cancelButtonTitle, onClick, otherButtons);
        }

        public static void ShowErrorAlert(string title, string message, Action callback = null)
        {
            if (Providers.AlertUtility != null)
            {
                Providers.AlertUtility.ShowErrorAlert(title, message, callback);
            }
        }

        public static void ShowAppError(Aquamonix.Mobile.Lib.Domain.Responses.ErrorResponseBody error, Action callback = null)
        {
            ExceptionUtility.Try(() =>
            {
                string title = StringLiterals.GenericErrorAlertTitle;
                string body = String.Empty;

                if (error != null)
                {
                    if (!String.IsNullOrEmpty(error.ResponseMessageShort))
                        title = error.ResponseMessageShort;
                    if (!String.IsNullOrEmpty(error.ResponseMessageLong))
                        body = error.ResponseMessageLong;
                }

                if (String.IsNullOrEmpty(body))
                {
                    if (error != null && error.ResponseCode != null && error.Process != null)
                        body = String.Format("{0} (ResponseCode:{1}, Process:{2})", StringLiterals.UnknownErrorText, error.ResponseCode, error.Process);
                    else
                        body = Domain.StringLiterals.UnknownErrorText;
                }

                #if DEBUG
                if (body != null)
                {
                    if (!String.IsNullOrEmpty(error.CommandId))
                        body += String.Format("\nCommand ID: {0}", error.CommandId);
                    if (error.ResponseCode != null && error.ResponseCode.GetValueOrDefault() != 0)
                        body += String.Format("\nResponse Code: {0}", error.ResponseCode);
                }
                #endif

                //dismiss error alert 
                if (error != null && !String.IsNullOrEmpty(body))
                {
                    ProgressUtility.Dismiss();
                }

                ShowErrorAlert(title, body, callback);
            });
        }

        public static void ShowProgressResponse(string title, ProgressResponse response)
        {
            ExceptionUtility.Try(() =>
            {
                if (response != null)
                {
                    if (response.IsFinal)
                    {
                        if (response.HasError)
                        {
                            //ShowAppError(response.ErrorBody);
                        }
                        else
                        {
                            string message = String.Empty;
                            if (response.Body != null)
                            {
                                message = String.Empty;
                                if (!String.IsNullOrEmpty(response.Body.Progress))
                                    message += "Progress: " + response.Body.Progress;

                                if (!String.IsNullOrEmpty(response.Body.ProgressDescription))
                                    message += "\nProgressDescription: " + response.Body.ProgressDescription;

                                if (response.Body.ProgressSpecific != null)
                                    message += "\nProgressSpecific: " + response.Body.ProgressSpecific;

                                if (!String.IsNullOrEmpty(response.Body.CommandId))
                                    message += "\nCommandId: " + response.Body.CommandId;
                            }

                            if (title == null)
                                title = String.Empty;

                            if (!response.IsSuccessful && !response.HasError)
                                ShowAlert(title, message);

                            LogUtility.LogMessage(message);
                        }
                    }
                }
            });
        }

        public static void ShowConfirmationAlert(string title, string message, Action<bool> callback = null, string okButtonText = StringLiterals.OkButtonText, string cancelButtonText = StringLiterals.CancelButtonText)
        {
            if (Providers.AlertUtility != null)
                Providers.AlertUtility.ShowConfirmationAlert(title, message, callback, okButtonText, cancelButtonText);
        }

        public static void RemoveAllAlerts()
        {
            if (Providers.AlertUtility != null)
                Providers.AlertUtility.RemoveAllAlerts();
        }

        internal static void ShowAppError(object errorBody)
        {
            throw new NotImplementedException();
        }

        internal static void ShowConfirmationAlert(object progressText, object promptText, Action<bool> p, object okButtonText, object cancelButtonText)
        {
            throw new NotImplementedException();
        }
    }
}
