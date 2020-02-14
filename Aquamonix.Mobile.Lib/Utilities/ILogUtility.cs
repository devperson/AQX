using System;
using System.Collections.Generic;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for platform-specific application-wide event & error logging. 
    /// </summary>
	public interface ILogUtility
	{
        /// <summary>
        /// Gets/sets a value indicating whether or not logging is enabled.
        /// </summary>
		bool Enabled { get; set;}

        /// <summary>
        /// Initialize anything needed to begin logging process.
        /// </summary>
		void StartLogging();

        /// <summary>
        /// Close out and dispose anything needed to clean up & stop logging.
        /// </summary>
		void EndLogging();

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logSeverity">Severity</param>
        /// <param name="extraData">Other accompanying data</param>
		void LogMessage(string message, LogSeverity logSeverity = LogSeverity.Info, Dictionary<string, string> extraData = null);

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        /// <param name="message">Message to log</param>
        /// <param name="logSeverity">Severity</param>
        /// <param name="extraData">Other accompanying data</param>
		void LogException(Exception exception, string message = null, LogSeverity logSeverity = LogSeverity.Warn, Dictionary<string, string> extraData = null);

        /// <summary>
        /// Logs an application (server) error. 
        /// </summary>
        /// <param name="error">Application error to log.</param>
		void LogAppError(Aquamonix.Mobile.Lib.Domain.Responses.ErrorResponseBody error); 

        /// <summary>
        /// Reads the current contents of the log.
        /// </summary>
        /// <returns>String contents of log</returns>
		string GetLogData();

        /// <summary>
        /// Clears all current log data.
        /// </summary>
		void ClearLogs();
	}

	public enum LogSeverity
	{
		Trace,
		Debug,
		Info,
		Track,
		Warn,
		Error
	}
}
