using System;
using Microsoft.Extensions.Logging;

namespace UrlTracker.Middleware
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(3000, LogLevel.Debug, "Incoming url detected: '{url}'")]
        public static partial void LogRequestDetected(this ILogger logger, string url);

        [LoggerMessage(3001, LogLevel.Debug, "Handling aborted: {reason}")]
        public static partial void LogAbortHandling(this ILogger logger, string reason);

        [LoggerMessage(3002, LogLevel.Debug, "Found an intercept of type '{interceptType}'")]
        public static partial void LogInterceptFound(this ILogger logger, Type interceptType);

        [LoggerMessage(3004, LogLevel.Debug, "Client error handling aborted: {reason}")]
        public static partial void LogAbortClientErrorHandling(this ILogger logger, string reason);

        [LoggerMessage(3005, LogLevel.Error, "An error occurred while processing a client error in the background")]
        public static partial void LogBackgroundProcessingFailure(this ILogger logger, Exception exception);
    }
}
