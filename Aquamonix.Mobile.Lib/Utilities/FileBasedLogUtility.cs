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
using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Implementation of ILogUtility that logs to the filesystem.
    /// </summary>
	public class FileBasedLogUtility : ILogUtility
	{
		private const LogSeverity DefaultLogSeverity = LogSeverity.Info; // Change this to DEBUG to see verbose Response Messages.
		private const bool LogMessageExtraData = true;
		private const bool LogExceptionExtraData = true;
		private const int DefaultMaxInfoSeverityLogMessageLength = 1000000;
		private const int DefaultMaxLogFiles = 3;

		private FileInfo _logFileInfo;
		private StreamWriter _logFileStreamWriter;
		private FileStream _logFileStream;
		private LogSeverity _configuredLogSeverity;


		private LogSeverity ConfiguredLogSeverity { get { return _configuredLogSeverity; } }

		private string LogFileFullPath { get; set; }

		private string LogFileName { get; set; }

		public bool Enabled { get; set;}


		public void StartLogging()
		{
			try
			{
				Enabled = true; 
				string logsDirectoryPath = FileUtility.GetLogDirectory();
				string pathSeparator = AppSettings.GetAppSettingValue(AppSettings.PathSeparatorAppSettingName, "/");
				logsDirectoryPath += pathSeparator;

				string logsDirectoryName = AppSettings.GetAppSettingValue(AppSettings.LoggingDirectoryNameAppSettingName, "Logs");
				//string logEmailRecipientsSeparator = AppSettings.GetAppSettingValue(AppSettings.LogEmailRecipientsSeparatorAppSettingName, ";");
				//string logEmailRecipientsString = AppSettings.GetAppSettingValue(AppSettings.LogEmailRecipientsAppSettingName, DefaultLogEmailRecipients);

				logsDirectoryPath += logsDirectoryName;
				CleanupLogsDirectory(logsDirectoryPath);

				string logFileExtension = AppSettings.GetAppSettingValue(AppSettings.LogFileExtensionAppSettingName, "log");
				string configuredLogSeverityStringValue = AppSettings.GetAppSettingValue(AppSettings.LogSeverityAppSettingName,
					DefaultLogSeverity.ToString());

				const bool ignoreCase = false;

				_configuredLogSeverity =
					(LogSeverity)Enum.Parse(typeof(LogSeverity), configuredLogSeverityStringValue, ignoreCase);

				//const string format = "M_dd_yyyy_h_mm_ss_tt";
				const string format = "M_dd_yyyy";
				string logFileName = DateTime.Now.ToString(format);
				logFileName = logFileName.Replace(pathSeparator, "_") + "." + logFileExtension;
				LogFileName = logFileName;

				string logFullPath = logsDirectoryPath + logFileName;
				LogFileFullPath = logFullPath;
				_logFileInfo = new FileInfo(logFullPath);

				DirectoryInfo directoryInfo = _logFileInfo.Directory;

				if (directoryInfo == null)
				{
					return;
				}

				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}

				if (_logFileInfo.Exists)
				{
					//_logFileInfo.Delete();
				}

				_logFileStream = File.Open(_logFileInfo.FullName, FileMode.Append, FileAccess.Write);
				_logFileStreamWriter = new StreamWriter(_logFileStream);
				LogMessage("Start Logging...", LogSeverity.Info);
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
		}

		public void EndLogging()
		{
			try
			{
				LogMessage("End Logging...", LogSeverity.Info);
				if (_logFileStream != null)
				{
					_logFileStream.Flush();
					_logFileStream.Close();
					_logFileStream = null;
				}
				_logFileStreamWriter = null;
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		public void LogMessage(string message, LogSeverity logSeverity = LogSeverity.Info, Dictionary<string, string> extraData = null)
        {
			try
			{
				if (logSeverity < _configuredLogSeverity)
				{
					return;
				}

				if (_configuredLogSeverity > LogSeverity.Debug)
				{
					message = message.SubstringIfLonger(DefaultMaxInfoSeverityLogMessageLength);
				}

				string finalMessage = message;

				if (extraData != null && LogMessageExtraData)
					finalMessage += System.Environment.NewLine + ExtraDataToString(extraData);

				WriteMessageInternal(logSeverity, finalMessage);

                //if (logSeverity >= LogSeverity.Track && LogToInsights)
                //	Insights.Track(string.Format("[{0}] {1}", logSeverity, message), extraData);
            }
			// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		public void LogException(Exception exception, string message = null, LogSeverity logSeverity = LogSeverity.Warn, Dictionary<string, string> extraData = null)
		{
			ExceptionUtility.Try(() =>
			{
				if (logSeverity < _configuredLogSeverity)
				{
					return;
				}

				string finalMessage = (message ?? "LogException") + System.Environment.NewLine;

				if (extraData != null && LogExceptionExtraData)
					finalMessage += ExtraDataToString(extraData);

				finalMessage += ExceptionToString(exception) + System.Environment.NewLine + System.Environment.NewLine;

				WriteMessageInternal(logSeverity, finalMessage);

				if (message != null)
				{
					if (extraData == null) extraData = new Dictionary<string, string>();
					extraData["LogMessage"] = message;
				}

				//if (logSeverity >= LogSeverity.Track && LogToInsights)
				//	Insights.Report(exception, extraData, logSeverity == LogSeverity.Error ? Xamarin.Insights.Severity.Error : Xamarin.Insights.Severity.Warning);

			});
		}

		public void LogAppError(Aquamonix.Mobile.Lib.Domain.Responses.ErrorResponseBody error)
		{
			string title = StringLiterals.GenericErrorAlertTitle;
			string body = null;

			if (error != null)
			{
				if (!String.IsNullOrEmpty(error.ResponseMessageShort))
					title = error.ResponseMessageShort;
				if (!String.IsNullOrEmpty(error.ResponseMessageLong))
					body = error.ResponseMessageLong;
			}

			if (String.IsNullOrEmpty(body))
			{
				if (error != null)
					body = String.Format("{0} (ResponseCode:{1}, Process:{2})", StringLiterals.UnknownErrorText, error.ResponseCode, error.Process);
				else
					body = Domain.StringLiterals.UnknownErrorText;
			}

			this.LogMessage(body, LogSeverity.Error); 
		}

		public string GetLogData()
		{
			return ExceptionUtility.Try<string>(() =>
			{
				/*
				return _logFileStreamReader.ReadToEnd(); 
				*/

				this.EndLogging();
				string output = File.ReadAllText(LogFileFullPath);
				this.StartLogging();

				return output; 
			});
		}

		public void ClearLogs()
		{
			this.EndLogging();
			_logFileInfo.Delete();
			this.StartLogging(); 
		}


		private void WriteMessageInternal(LogSeverity severity, string message)
        {
			if (this.Enabled)
			{
				try
				{
					if (severity >= LogSeverity.Warn)
						System.Diagnostics.Debug.WriteLine("[" + severity + "] " + message);
					//Console.Error.WriteLine("[" + severity + "] " + message);
					else if (severity >= LogSeverity.Info)
						System.Diagnostics.Debug.WriteLine("[" + severity + "] " + message);
					//Console.Out.WriteLine("[" + severity + "] " + message);
					else //if(severity >= LogSeverity.Debug)
						Debug.WriteLine("[" + severity + "] " + message);
					//else
					//	Trace.WriteLine("[" + severity + "] " + message);

					if (_logFileStreamWriter != null)
						_logFileStreamWriter.WriteLine(DateTime.Now + " [" + severity + "] " + message);
                }
				catch { }
			}

			// _logFileStreamWriter.Flush();
		}
		   
		private string ExtraDataToString(Dictionary<string, string> extraData)
		{
			var extraDataString = "  ***** Extra Data *****" + System.Environment.NewLine;
			extraData.OrderBy(pair => pair.Key).ToList().ForEach(
				pair => extraDataString += "  * " + pair.Key + "  :  " +
				pair.Value + //.SubstringIfLonger(DefaultMaxInfoSeverityLogMessageLength) + 
				System.Environment.NewLine);
			return extraDataString;
		}

		private string ExceptionToString(Exception exception)
		{
			var ae = (exception as AggregateException);
			exception = ae != null ? ae.Flatten() : exception;

			return ExceptionToStringRecursive(exception);
		}

		private string ExceptionToStringRecursive(Exception exception)
		{
			var s = "Exception: " + exception.GetType().ToString() + System.Environment.NewLine +
				"Message: " + exception.Message; 

			if (!(exception is ObjectDisposedException))
				s +=  System.Environment.NewLine +
					exception.StackTrace;

			if (exception.InnerException != null)
				return ExceptionToString(exception.InnerException) + System.Environment.NewLine +
				" ----------- Inner Exception ------------ " + System.Environment.NewLine +
				s;

			return s;
		}

		private void CleanupLogsDirectory(string logsDirectoryPath)
		{
			var logsDirectoryInfo = new DirectoryInfo(logsDirectoryPath);
			if (!logsDirectoryInfo.Exists)
			{
				return;
			}
			var logFiles = new List<FileInfo>(logsDirectoryInfo.GetFiles());
			logFiles.Sort(FileInfoByModifiedDateComparer.Instance);
			int maxLogFiles = AppSettings.GetAppSettingValue(AppSettings.MaxLogFilesAppSettingName, DefaultMaxLogFiles);
			for (var i = (maxLogFiles - 1); i < (logFiles.Count - 1); i++)
			{
				FileInfo logFileInfo = logFiles[i];
				ExceptionUtility.Try(() =>
				{
					logFileInfo.Delete();
				});
			}
		}


		public class FileInfoByModifiedDateComparer : IComparer<FileInfo>
		{
			public static FileInfoByModifiedDateComparer Instance = new FileInfoByModifiedDateComparer();

			public int Compare(FileInfo x, FileInfo y)
			{
				return x.LastWriteTime.CompareTo(y.LastWriteTime);
			}
		}
	}
}

