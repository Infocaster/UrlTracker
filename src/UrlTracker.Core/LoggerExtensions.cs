using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Logging;

namespace UrlTracker.Core
{
    [ExcludeFromCodeCoverage]
    internal static class LoggerExtensions
    {
        public static void LogApplyMigration<T>(this ILogger logger, string migrationName)
        {
            logger.Info<T>("Applying migration {migrationName}", migrationName);
        }

        public static void LogSkipStep<T>(this ILogger logger, string step, string reason)
        {
            logger.Info<T>("Skip step {step}: {reason}", step, reason);
        }

        public static void LogStepSuccess<T>(this ILogger logger, string step)
        {
            logger.Info<T>("Step {step} succeeded", step);
        }

        public static void LogResults<T>(this ILogger logger, int resultCount)
        {
            logger.Debug<T>("{source} found {resultCount} results", typeof(T), resultCount);
        }

        public static void LogParameters<T>(this ILogger logger, string culture, int? rootnodeid, List<string> urls)
        {
            logger.Verbose<T>("No longer available parameters: culture: {culture}, rootnodeid: {rootnodeid}, urls: {urls}", culture, rootnodeid, urls);
        }
    }
}
