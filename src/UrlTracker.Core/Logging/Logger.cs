using System;
using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Core.Logging
{
    public interface ILogger
        : Umbraco.Core.Logging.ILogger
    { }

    [ExcludeFromCodeCoverage]
    internal class Logger
        : ILogger
    {
        private readonly Umbraco.Core.Logging.ILogger _logger;
        private readonly IConfiguration<UrlTrackerSettings> _configuration;

        public Logger(Umbraco.Core.Logging.ILogger logger, IConfiguration<UrlTrackerSettings> configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private bool LoggingEnabled => _configuration.Value.LoggingEnabled;

        public void Debug(Type reporting, string message)
        {
            if (LoggingEnabled) _logger.Debug(reporting, message);
        }

        public void Debug(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Debug(reporting, messageTemplate, propertyValues);
        }

        public void Error(Type reporting, Exception exception, string message)
        {
            if (LoggingEnabled) _logger.Error(reporting, exception, message);
        }

        public void Error(Type reporting, Exception exception)
        {
            if (LoggingEnabled) _logger.Error(reporting, exception);
        }

        public void Error(Type reporting, string message)
        {
            if (LoggingEnabled) _logger.Error(reporting, message);
        }

        public void Error(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Error(reporting, exception, messageTemplate, propertyValues);
        }

        public void Error(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Error(reporting, messageTemplate, propertyValues);
        }

        public void Fatal(Type reporting, Exception exception, string message)
        {
            if (LoggingEnabled) _logger.Fatal(reporting, exception, message);
        }

        public void Fatal(Type reporting, Exception exception)
        {
            if (LoggingEnabled) _logger.Fatal(reporting, exception);
        }

        public void Fatal(Type reporting, string message)
        {
            if (LoggingEnabled) _logger.Fatal(reporting, message);
        }

        public void Fatal(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Fatal(reporting, exception, messageTemplate, propertyValues);
        }

        public void Fatal(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Fatal(reporting, messageTemplate, propertyValues);
        }

        public void Info(Type reporting, string message)
        {
            if (LoggingEnabled) _logger.Info(reporting, message);
        }

        public void Info(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Info(reporting, messageTemplate, propertyValues);
        }

        public bool IsEnabled(Type reporting, Umbraco.Core.Logging.LogLevel level)
        {
            return _logger.IsEnabled(reporting, level);
        }

        public void Verbose(Type reporting, string message)
        {
            if (LoggingEnabled) _logger.Verbose(reporting, message);
        }

        public void Verbose(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Verbose(reporting, messageTemplate, propertyValues);
        }

        public void Warn(Type reporting, string message)
        {
            if (LoggingEnabled) _logger.Warn(reporting, message);
        }

        public void Warn(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Warn(reporting, messageTemplate, propertyValues);
        }

        public void Warn(Type reporting, Exception exception, string message)
        {
            if (LoggingEnabled) _logger.Warn(reporting, exception, message);
        }

        public void Warn(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (LoggingEnabled) _logger.Warn(reporting, exception, messageTemplate, propertyValues);
        }
    }
}
