using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Logging;
using UrlTracker.Core.Models;

namespace UrlTracker.Web
{
    [ExcludeFromCodeCoverage]
    internal static class LoggerExtensions
    {
        public static void LogUrlTrackerDisabled<T>(this ILogger logger)
        {
            logger.Debug<T>("Url tracker is disabled by config");
        }

        public static void LogEventPublished<T>(this ILogger logger, Type eventType, Type source)
        {
            logger.Verbose<T>("{eventType} event published", eventType, source);
        }

        public static void LogSubscriberError<T>(this ILogger logger, Exception exception, Type subscriberType, Type eventType)
        {
            logger.Error<T>(exception, "{subscriberType} threw an exception while handling {eventType} event", subscriberType, eventType);
        }

        public static void LogRequestDetected<T>(this ILogger logger, string url)
        {
            logger.Verbose<T>("Incoming url detected: '{url}'", url);
        }

        public static void LogAbortHandling<T>(this ILogger logger, string reason)
        {
            logger.Verbose<T>("Handling aborted: {reason}", reason);
        }

        public static void LogInterceptFound<T>(this ILogger logger, Type interceptType)
        {
            logger.Debug<T>("Found an intercept of type '{interceptType}'", interceptType);
        }

        public static void LogRegisteredEventHandlers<T>(this ILogger logger)
        {
            logger.Debug<T>("Registered eventhandlers");
        }

        public static void LogAbortClientErrorHandling<T>(this ILogger logger, string reason)
        {
            logger.Verbose<T>("Client error handling aborted: {reason}", reason);
        }

        public static void LogInterceptCancelled<T>(this ILogger logger, string reason, ShallowRedirect redirect)
        {
            logger.Debug<T>("Intercept cancelled: {reason}", reason, redirect);
        }

        public static void LogInterceptPerformed<T>(this ILogger logger, string targetUrl)
        {
            logger.Info<T>("Redirect request to: {targetUrl}", targetUrl);
        }

        public static void LogStart<T>(this ILogger logger)
        {
            logger.Verbose<T>("Filter incoming url with {source}", typeof(T));
        }

        public static void LogPathIsReserved<T>(this ILogger logger)
        {
            logger.Debug<T>("Incoming url is reserved by umbraco settings.");
        }
    }
}
