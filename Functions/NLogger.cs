using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ArbibetProgram.Functions
{
	public enum EventLevel
	{
		All,
		Verbose,
		Trace,
		Debug,
		Info,
		Notice,
		Warn,
		Error,
		Severe,
		Critical,
		Alert,
		Fatal,
		Emergency
	}
	public static class NLogger
	{
		private static Logger prv_Logger = null;    // static instance of NLog
		private static string prv_WhichMode = "";

		public static void SetupLogger()
		{
			// preprocessor commands know which mode we're running in
			#if DEBUG
				prv_WhichMode = "_Debug";
			#endif

			// not entierly sure why we need to do this twice, but anyway...
			LogManager.Configuration.Variables["WhichMode"] = prv_WhichMode;

			prv_Logger = LogManager.GetCurrentClassLogger();
			prv_Logger.LoggerReconfigured += Logger_LoggerReconfigured;

			LogManager.Configuration.Variables["WhichMode"] = prv_WhichMode;
			LogManager.ReconfigExistingLoggers();
		}

		private static void Logger_LoggerReconfigured(object sender, EventArgs e)
		{
			try
			{
				LogManager.Configuration.Variables["WhichMode"] = prv_WhichMode;
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
		}

		public static void Flush()
		{
			LogManager.Flush();
		}

		public static void LogError(Exception prv_Exception)
		{
			prv_Logger.Fatal(prv_Exception.Message);
			//if (prv_Exception.Message.Contains("One or more errors occurred."))
			if (prv_Exception.InnerException != null)
			{
				prv_Logger.Fatal(prv_Exception.InnerException.Message);
			}
			prv_Logger.Error(prv_Exception.StackTrace);
			Flush();
		}

		public static void Log(EventLevel prm_EventLevel, string prm_Message)
		{
			switch (prm_EventLevel)
			{
				case EventLevel.All:
					break;

				case EventLevel.Verbose:
					prv_Logger.Verbose(prm_Message);
					break;
				case EventLevel.Trace:
					prv_Logger.Trace(prm_Message);
					break;
				case EventLevel.Debug:
					prv_Logger.Debug(prm_Message);
					break;
				case EventLevel.Info:
					prv_Logger.Info(prm_Message);
					break;
				case EventLevel.Notice:
					prv_Logger.Notice(prm_Message);
					break;
				case EventLevel.Warn:
					prv_Logger.Warn(prm_Message);
					break;
				case EventLevel.Error:
					prv_Logger.Error(prm_Message);
					break;
				case EventLevel.Severe:
					prv_Logger.Severe(prm_Message);
					break;
				case EventLevel.Critical:
					prv_Logger.Critical(prm_Message);
					break;
				case EventLevel.Alert:
					prv_Logger.Alert(prm_Message);
					break;
				case EventLevel.Fatal:
					prv_Logger.Fatal(prm_Message);
					break;
				case EventLevel.Emergency:
					prv_Logger.Emergency(prm_Message);
					break;
			}
		}
	}
}
