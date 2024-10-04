using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using UrlTracker.Core.Models;

namespace UrlTracker.Web
{
    [ExcludeFromCodeCoverage]
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(2009, LogLevel.Debug, "Intercept cancelled: {reason}, {redirect}")]
        public static partial void LogInterceptCancelled(this ILogger logger, string reason, Redirect redirect);

        [LoggerMessage(2010, LogLevel.Information, "Redirect request to: {targetUrl}")]
        public static partial void LogRequestRedirected(this ILogger logger, string targetUrl);

        [LoggerMessage(2011, LogLevel.Debug, "Filter incoming url with {source}")]
        public static partial void LogStart(this ILogger logger, Type source);

        [LoggerMessage(2012, LogLevel.Debug, "Incoming url is reserved by umbraco settings.")]
        public static partial void LogPathIsReserved(this ILogger logger);

        [LoggerMessage(2013, LogLevel.Information, "Response converted to 410")]
        public static partial void LogRequestConvertedToGone(this ILogger logger);

        [LoggerMessage(2014, LogLevel.Warning, "Last chance handler invoked for intercept of type {interceptType}. Did you forget to register a handler?")]
        public static partial void LogLastChance(this ILogger logger, Type interceptType);
    }
}
