using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.Linq;

using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Extensions;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Static helper class for application-wide logging.
    /// </summary>
	public static class LogUtility
	{
		public static bool Enabled
		{
			get
			{
				if (Providers.LogUtility != null)
					return Providers.LogUtility.Enabled;

				return false;
			}
			set
			{
				if (Providers.LogUtility != null)
					Providers.LogUtility.Enabled = value;
			}
		}

		public static void StartLogging()
		{
			ExceptionUtility.Try(() => { 
                if (Providers.LogUtility != null)
                    Providers.LogUtility.StartLogging(); 
            }); 
		}

		public static void EndLogging()
		{
			ExceptionUtility.Try(() => {
                if (Providers.LogUtility != null)
                    Providers.LogUtility.EndLogging(); 
            });
		}

		public static void LogMessage(string message, LogSeverity logSeverity = LogSeverity.Info, Dictionary<string, string> extraData = null)
		{
			ExceptionUtility.Try(() => {
                if (Providers.LogUtility != null)
                    Providers.LogUtility.LogMessage(message, logSeverity, extraData); 
            });
        }

		public static void LogException(Exception exception, string message = null, LogSeverity logSeverity = LogSeverity.Warn, Dictionary<string, string> extraData = null)
		{
			try { 
				if (Providers.LogUtility != null)
					Providers.LogUtility.LogException(exception, message, logSeverity, extraData); 
			}
			catch { }
		}

		public static void LogAppError(Aquamonix.Mobile.Lib.Domain.Responses.ErrorResponseBody error)
		{
			ExceptionUtility.Try(() =>
			{
				if (Providers.LogUtility != null)
					Providers.LogUtility.LogAppError(error);
			});
		}

		public static string GetLogData()
		{
			if (Providers.LogUtility != null)
				return Providers.LogUtility.GetLogData();

			return String.Empty;
		}

		public static void ClearLogs()
		{
			if (Providers.LogUtility != null)
				Providers.LogUtility.ClearLogs();
		}
	}
}

