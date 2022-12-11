using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace UrlTracker.Core
{
    [ExcludeFromCodeCoverage]
    internal static class LoggerExtensions
    {
        public static void LogApplyMigration(this ILogger logger, string migrationName)
        {
            logger.LogInformation(new EventId(1001), "Applying migration {migrationName}", migrationName);
        }

        public static void LogSkipStep(this ILogger logger, string step, string reason)
        {
            logger.LogInformation(new EventId(1002), "Skip step {step}: {reason}", step, reason);
        }

        public static void LogStepSuccess(this ILogger logger, string step)
        {
            logger.LogInformation(new EventId(1003), "Step {step} succeeded", step);
        }

        public static void LogResults<T>(this ILogger logger, int resultCount)
        {
            logger.LogDebug(new EventId(1004), "{source} found {resultCount} results", typeof(T), resultCount);
        }

        public static void LogParameters(this ILogger logger, string? culture, int? rootnodeid, List<string> urls)
        {
            logger.LogDebug(new EventId(1005), "No longer available parameters: culture: {culture}, rootnodeid: {rootnodeid}, urls: {urls}", culture, rootnodeid, urls);
        }

        public static void LogRedactionScoreNotFound(this ILogger logger, Guid key)
        {
            logger.LogWarning(new EventId(1006), "Could not find a redaction score for given key: {key}", key);
        }

        public static void LogClassificationFailed(this ILogger logger)
        {
            logger.LogInformation(new EventId(1007), "Classification of the url failed. falling back on default classification...");
        }
    }
}
