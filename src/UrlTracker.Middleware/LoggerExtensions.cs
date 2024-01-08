using System;
using Microsoft.Extensions.Logging;

namespace UrlTracker.Middleware
{
    internal static class LoggerExtensions
    {
        public static void LogRequestDetected(this ILogger logger, string url)
            => logger.LogDebug(new EventId(3000), "Incoming url detected: '{url}'", url);

        public static void LogAbortHandling(this ILogger logger, string reason)
            => logger.LogDebug(new EventId(3001), "Handling aborted: {reason}", reason);

        public static void LogInterceptFound(this ILogger logger, Type interceptType)
            => logger.LogDebug(new EventId(3002), "Found an intercept of type '{interceptType}'", interceptType);

        public static void LogAbortClientErrorHandling(this ILogger logger, string reason)
            => logger.LogDebug(new EventId(3004), "Client error handling aborted: {reason}", reason);

        public static void LogBackgroundProcessingFailure(this ILogger logger, Exception exception)
            => logger.LogError(new EventId(3005), "An error occurred while processing a client error in the background");
    }
}
