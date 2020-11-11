using InfoCaster.Umbraco.UrlTracker.Settings;
using System;
using Umbraco.Core.Logging;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
	public class UrlTrackerNewLoggingHelper : IUrlTrackerNewLoggingHelper
	{
		private readonly ILogger _logger;
		private readonly IUrlTrackerNewSettings _urlTrackerSettings;

		public UrlTrackerNewLoggingHelper(
			ILogger logger,
			IUrlTrackerNewSettings urlTrackerSettings)
		{
			_logger = logger;
			_urlTrackerSettings = urlTrackerSettings;
		}

		public void LogException(Exception ex)
		{
			_logger.Error(typeof(UrlTrackerComponent), ex);
		}

		public void LogInformation(string message, params object[] args)
		{
			LogInformation(string.Format(message, args));
		}

		public void LogInformation(string message)
		{
			if (_urlTrackerSettings.LoggingEnabled())
				_logger.Debug(typeof(UrlTrackerComponent), message);
		}
    }
}