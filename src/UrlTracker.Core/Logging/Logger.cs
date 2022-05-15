using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Core.Logging
{
    public interface ILogger<TSource>
        : Microsoft.Extensions.Logging.ILogger<TSource>
    { }

    [ExcludeFromCodeCoverage]
    internal class Logger<TSource>
        : ILogger<TSource>
    {
        private readonly Microsoft.Extensions.Logging.ILogger<TSource> _logger;
        private readonly IOptions<UrlTrackerSettings> _options;

        public Logger(Microsoft.Extensions.Logging.ILogger<TSource> logger, IOptions<UrlTrackerSettings> options)
        {
            _logger = logger;
            _options = options;
        }

        public IDisposable BeginScope<TState>(TState state)
            => _logger.BeginScope<TState>(state);

        public bool IsEnabled(LogLevel logLevel)
            => _logger.IsEnabled(logLevel) && _options.Value.LoggingEnabled;

        public void Log<TState>(LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel)) _logger.Log<TState>(logLevel, eventId, state, exception, formatter);
        }
    }
}
