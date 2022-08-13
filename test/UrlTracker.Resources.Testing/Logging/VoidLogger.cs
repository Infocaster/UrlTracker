using System;
using UrlTracker.Core.Logging;

namespace UrlTracker.Resources.Testing.Logging
{
    public class VoidLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new Disposable();
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        { }

        private class Disposable : IDisposable
        {
            public void Dispose()
            { }
        }
    }
}
