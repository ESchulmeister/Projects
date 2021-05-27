using System;
using System.Diagnostics;
using System.Security;

namespace Empire.Shared.Utilities
{
    public class EventLogAdapter
    {
		#region Constants
		public const string WindowsEventLogSource = "EmpireCloudIO";
		public const string WindowsEventDefaultSource = "Application";
		public const string WindowsEventLogLog = "Application";


		public const int WindowsEventLogCommonErrorNumber = 8888;
		#endregion

		#region Methods
		public void WriteErrorEntry(string sErrorMessage)
		{
			bool bLogToEventLog = ConfigReader.Default.Read("LogErrorsToWindowsEventLog", true);
			if (!bLogToEventLog)
			{
                return;
			}

			string sSource = EventLogAdapter.WindowsEventLogSource;
			string sLog = EventLogAdapter.WindowsEventLogLog;
			string sEvent = sErrorMessage;

			// this will generate a security exception since the SecurityLog can not be checked at this level
			try
			{
				if (!EventLog.SourceExists(sSource))
				{
					EventLog.CreateEventSource(sSource, sLog);
				}
			}
			catch (SecurityException) { }
			catch
			{ 
				// we'll only try this if a SecurityException was not generated
				try 
				{ 
					EventLog.CreateEventSource(sSource, sLog); 
				} 
				catch { }
			}
			try
			{
				EventLog.WriteEntry(sSource, sEvent);
			}
			catch
			{
				sSource = EventLogAdapter.WindowsEventDefaultSource;
			}
			EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, EventLogAdapter.WindowsEventLogCommonErrorNumber);
		}

		#endregion
	}
}
