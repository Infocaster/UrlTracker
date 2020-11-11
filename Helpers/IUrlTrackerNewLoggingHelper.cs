using System;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
	public interface IUrlTrackerNewLoggingHelper
	{
		void LogException(Exception ex);
		void LogInformation(string message, params object[] args);
		void LogInformation(string message);
	}
}