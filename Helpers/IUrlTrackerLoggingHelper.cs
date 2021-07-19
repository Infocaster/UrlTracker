using System;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
	public interface IUrlTrackerLoggingHelper
	{
		void LogException(Exception ex);
		void LogInformation(string message, params object[] args);
		void LogInformation(string message);
	}
}