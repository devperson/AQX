using System;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Domain
{
	public static class StringLiterals
	{
		public const string Alerts = "Alerts";
		public const string Stations = "Stations";
		public const string Programs = "Programs";
		public const string Schedules = "Schedules";
		public const string Circuits = "Circuits";
		public const string Disabled = "Disabled";
        public const string DismissAll = "DismissAll";
		public const string Error = "Error";
		public const string Update = "Update";
		public const string Start = "Start";
		public const string Stop = "Stop";
		public const string UserSettingsTitle = "Settings";
		public const string LogoutButtonText = "Logout";
		public const string SupportButtonText = "Contact Support";
		public const string UnknownErrorText = "An unknown error has occurred.";
		public const string UsernameTextFieldPlaceholder = "Email";
		public const string PasswordTextFieldPlaceholder = "Password";
		public const string ServerUriTextFieldPlaceholder = "Connect to";
		public const string CancelButtonText = "Cancel";
		public const string OkButtonText = "Yes";
        public const String YesButtonText = "Dismiss All";
		public const string SelectAllButtonText = "Select All";
		public const string DoneButtonText = "Done";
		public const string ConnectionError = "Connection Error";
		public const string GenericErrorAlertTitle = "Error";
		public const string AlertConfirmButtonText = "OK";
		public const string Devices = "Devices";
		public const string Disable = "Disable";
        public const string StandBy = "Stand By";
        public const string EndGun = "End Gun";
      
        public const string Aux1 = "Aux 1";
        public const string Aux2 = "Aux 2";
        public const string Fwd = "Fwd";
        public const string Rev = "Rev";
        public const string Auto = "Auto";
        public const string Reverse = "Reverse"; //do not change
        public const string Forward = "Forward"; //do not change
        public const string Wet = "Wet";
        public const string Dry = "Dry";
        public const string Speed = "Speed";
        public const string Watering = "Watering";
		public const string AutoStop = "Auto Stop";
		public const string AutoRev = "Auto Rev";
		public const string StepNumber = "Step1";
        public const string Angle = "50 deg";
        public const string MM = "mm";
        public const string EndGunE = "E";
        public const string Aux1A1 = "A1";
        public const string Aux2A2 = "A2";
        public const string PivotPrograms = "PivotPrograms";
        public const string TimerSetButtonText = "Set";
		public const string TimerTitle = "Timer";
		public const string TurnOnPermanently = "Turn on Permanently";
		public const string On = "On";
        public const string Sensors = "Sensors";
        public const string Off = "Off";
		public const string SearchTextPlaceholder = "Search";
		public const string SupportRequestSubject = "Aquamonix App Support";
		public const string DeviceNotFoundErrorMessage = "Sorry, device {0} could not be found in the database.";
		public const string DeviceNotFoundErrorTitle = "Device Not Found";
		public const string TestStationConfirmationTitle = "Test Station";
		public const string TestStationConfirmationMessage = "Test {0} ({1}) without running any pumps or master valves? (Station may start watering if the main line is pressurised)";
		public const string StartStationConfirmationTitle = "Start Station";
        public const string Alert = "Alert";
		public const string StartStationConfirmationMessage = "Start {0} ({1})? (This will stop the previous station started manually)";
		public const string TestButtonText = "Test";
		public const string WateringButtonText = "Watering";
		public const string StopTestButtonText = "Stop Test";
		public const string StartButtonText = "Start";
		public const string StopButtonText = "Stop";
		public const string Starting = "Starting";
        public const string Stopping = "Stopping";
        public const string Stopped = "Stopped"; //do not change
        public const string Waiting = "Waiting"; //do not change
        public const string ProgramStopped = "Program Stopped";
        public const string Running = "Running"; //do not change
        public const string ProgramRunning = "Program Running"; 
        public const string StartProgramConfirmationTitle = "Start Program";
		public const string StopProgramConfirmationTitle = "Stop Program";
		public const string NoMailSupportAlertTitle = "No Mail Support";
		public const string NoMailSupportAlertMessage = "Sorry, your device is not set up to send mail. Please contact email support: {0}";
		public const string EnableDeviceConfirmationTitle = "Enable Device";
		public const string DisableDeviceConfirmationTitle = "Disable Device";
		public const string ScaleProgramsConfirmationTitle = "Scale Programs";
		public const string StartWatering = "Start Watering";
		public const string StartCircuits = "Start Circuits";
		public const string SetTimer = "Set Timer";
		public const string SelectPumps = "Select Pumps";
		public const string SchedulesEmptyMessage = "This device has no schedules to display at the moment";
		public const string StationsEmptyMessage = "This device has no stations to display at the moment";
		public const string ProgramsEmptyMessage = "This device has no programs to display at the moment";
		public const string DevicesEmptyMessage = "There are no devices to display at the moment";
		public const string CircuitsEmptyMessage = "This device has no circuits to display at the moment";
		public const string AlertsEmptyMessage = "This device has no alerts to display at the moment";
        public const string AuthFailure = "Authentication Failure";
        public const string AuthFailureMessage = "Failed to log in/authenticate with server";
        public const string UserNotAuthenticated = "User not authenticated.";
		public const string ConnectionTimeout = "Connection Timeout";
		public const string UnableToEstablishConnection = "Unable to establish a connection with the server.";
		public const string ConnectionUnavailable = "Connection Unavailable";
		public const string ConnectionLost = "Connection to the server has been lost.";
		public const string TryAgain = "We can try again to reconnect, or go back to Settings.";
        public const string Reconnecting = "Reconnecting";
        public const string Connected = "Connected";
        public const string ServerNotAvailable = "Server not Available";
        public const string PleaseWaitReconnect = "Please wait while we try to reconnect you.";
		public const string UpdatingText = "Updating...";
		public const string Today = "Today";
		public const string Yesterday = "Yesterday";
        public const string Pivot = "Pivot";
        public const string EmailText = "Please send us this email if you have a question, would like to provide some feedback, or if you experience any difficulty using this app.You can attach screenshots of your device to this email if they provide information related to your support request.You can also add any comments below to help explain the issue or ask a question. We have included some technical information in this email to help us assist and improve our products.\n\n" +
			"Your comments:\n\n\n\n" +
			"Device info: \n\n";
        //added
        public const string Dismiss = "Dismiss";
        public static string FormatScaleProgramsConfirmationMessage(int value)
		{
			return String.Format("Scale programs to {0}% of their normal run time on all controllers in this group? (This will not affect programs already running)", value); 
		}

		public static string FormatDeviceNotFoundErrorMessage(string deviceId)
		{
			return String.Format(DeviceNotFoundErrorMessage, deviceId);
		}

		public static string FormatTestStationConfirmationMessage(IEnumerable<string> stationNames, IEnumerable<string> stationNumbers)
		{
			return String.Format(TestStationConfirmationMessage, StringUtility.ToCommaDelimitedList(stationNames, "station", "stations"), StringUtility.ToCommaDelimitedList(stationNumbers, "station", "stations"));
		}

		public static string FormatStartStationConfirmationMessage(IEnumerable<string> stationNames, IEnumerable<string> stationNumbers)
		{
			return String.Format(StartStationConfirmationMessage, StringUtility.ToCommaDelimitedList(stationNames, "station", "stations"), StringUtility.ToCommaDelimitedList(stationNumbers, "station", "stations"));
		}

		public static string FormatNumberSelected(int number)
		{
			return String.Format("{0} Selected", number); 
		}

        public static string FormatStartStopProgramConfirmationMessage(bool start, string programName, string programId)
		{
			string action = start ? "Start" : "Stop";
			return String.Format("{0} Program {1} (program {2})?", action, programName, programId);
		}
        public static string FormatDismissAlertsConfirmationMessage()
        {
            string Alert = "Mark all new alrms as read?";
            return String.Format( Alert);
        }
        //added//
        public static string DismissAllAlertsConfirmationMessage()
        {
            string Alert = "Dismiss all alerts?";
            return String.Format(Alert);
        }
        //added
        public static string ActiveAlertsNotAccessForDissmiss()
        {
            string Alert = "You do not have permission to dismiss some active alerts";
            return String.Format(Alert);
        }

        public static string FormatProgramsAtPercent(int percent)
		{
			return String.Format("Programs at {0}%", percent);
		}

		public static string FormatDisabledUntil(DateTime? disabledUntil)
		{
			return String.Format("Disabled {0}", FormatTimeUntil(disabledUntil)); 
		}

		public static string FormatEnableDisableDeviceConfirmationMessage(bool enable)
		{
			return String.Format("Are you sure you want to {0} the device?", enable ? "enable" : "disable"); 
		}

		public static string FormatTimeUntil(DateTime? time)
		{
			if (time == null)
				return "indefinitely";
			else {
				var ts = time.Value.Subtract(DateTime.Now);
				int days = 0;
				int hours = 0;
				int minutes = 0;
				int seconds = (int)ts.TotalSeconds;

				if (ts.TotalDays >= 1)
				{
					days = (int)Math.Floor(ts.TotalDays);
					seconds -= 86400 * days;
				}

				if (seconds >= 3600)
				{
					hours = (int)Math.Floor((double)(seconds / 3600));
					seconds -= (hours * 3600); 
				}

				if (seconds >= 60)
				{
					minutes = (int)Math.Floor((double)(seconds / 3600));
					seconds -= (minutes * 60);
				}

				var sb = new System.Text.StringBuilder();
				if (days > 0)
					sb.Append(String.Format("{0} {1} ", days, ((days > 1) ? "days" : "day"))); 

				if (hours > 0)
					sb.Append(String.Format("{0} {1} ", hours, ((hours > 1) ? "hours" : "hour")));

				if (minutes > 0)
					sb.Append(String.Format("{0} {1} ", minutes, ((minutes > 1) ? "minutes" : "minute")));

				return "for " + sb.ToString().Trim();
			}
		}
	}
}
