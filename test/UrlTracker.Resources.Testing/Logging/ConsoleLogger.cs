using System;
using UrlTracker.Core.Logging;

namespace UrlTracker.Resources.Testing.Logging
{
    public class ConsoleLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new Disposable();
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(logLevel.ToString(), exception, formatter(state, exception));
        }

        private void Log(string severity, Exception exception, string message)
        {
            Console.WriteLine($"[{DateTime.UtcNow} {severity,10}] <{typeof(T)}> {exception} | {message}");
        }

        private class Disposable : IDisposable
        {
            public void Dispose()
            { }
        }
    }
}
