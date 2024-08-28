using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace UrlTracker.Core
{
    [ExcludeFromCodeCoverage]
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(1001, LogLevel.Information, "Applying migration {migrationName}")]
        public static partial void LogApplyMigration(this ILogger logger, string migrationName);

        [LoggerMessage(1002, LogLevel.Information, "Skip step {step}: {reason}")]
        public static partial void LogSkipStep(this ILogger logger, string step, string reason);

        [LoggerMessage(1003, LogLevel.Information, "Step {step} succeeded")]
        public static partial void LogStepSuccess(this ILogger logger, string step);

        [LoggerMessage(1004, LogLevel.Debug, "{source} found {resultCount} results")]
        public static partial void LogResults(this ILogger logger, Type source, int resultCount);

        [LoggerMessage(1006, LogLevel.Warning, "Could not find a redaction score for given key: {key}")]
        public static partial void LogRedactionScoreNotFound(this ILogger logger, Guid key);

        [LoggerMessage(1007, LogLevel.Information, "Classification of the url failed. falling back on default classification...")]
        public static partial void LogClassificationFailed(this ILogger logger);
    }
}
