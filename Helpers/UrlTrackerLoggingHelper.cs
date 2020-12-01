using InfoCaster.Umbraco.UrlTracker.Settings;
using System;
using Umbraco.Core.Logging;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
	public class UrlTrackerLoggingHelper : IUrlTrackerLoggingHelper
	{
		private readonly ILogger _logger;
		private readonly IUrlTrackerSettings _urlTrackerSettings;

		public UrlTrackerLoggingHelper(
			ILogger logger,
			IUrlTrackerSettings urlTrackerSettings)
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