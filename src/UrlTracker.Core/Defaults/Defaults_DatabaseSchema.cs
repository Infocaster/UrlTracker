using System;

namespace UrlTracker.Core
{
    public static partial class Defaults
    {
        public static class DatabaseSchema
        {
            public const string TableNamePrefix = "urltracker";
            public const string MigrationName = "UrlTracker";

            public static class Tables
            {
                public const string Redirect = TableNamePrefix + "Redirect";
                public const string ClientError = TableNamePrefix + "ClientError";
                public const string Referrer = TableNamePrefix + "Referrer";
                public const string ClientError2Referrer = TableNamePrefix + "ClientError2Referrer";
            }

            public static class AggregateColumns
            {
                public const string TotalOccurrences = "totalOccurrances";
                public const string MostRecentOccurrence = "mostRecentOccurrence";
                public const string MostCommonReferrer = "mostCommonReferrer";
            }

            public static class ClientErrorStrategies
            {
                public static readonly Guid NotFound = new Guid("69b34892-0c2f-48c5-9739-28b71f1dc675");
                public static readonly Guid NoLongerExists = new Guid("a6f75e6e-aa46-4324-9ee1-24d1db26e9a6");
            }
        }
    }
}
