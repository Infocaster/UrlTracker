using System;

namespace UrlTracker.Core
{
    public static partial class Defaults
    {
        /// <summary>
        /// Constants related to database storage
        /// </summary>
        public static class DatabaseSchema
        {
            /// <summary>
            /// The prefix that all tablenames share
            /// </summary>
            public const string TableNamePrefix = "urltracker";

            /// <summary>
            /// The name of the migration plan for the URL Tracker
            /// </summary>
            public const string MigrationName = "UrlTracker";

            /// <summary>
            /// The names of all the tables
            /// </summary>
            public static class Tables
            {
                /// <summary>
                /// The name of the redirects table
                /// </summary>
                public const string Redirect = TableNamePrefix + "Redirect";

                /// <summary>
                /// The name of the client errors table
                /// </summary>
                public const string ClientError = TableNamePrefix + "ClientError";

                /// <summary>
                /// The name of the referrers table
                /// </summary>
                public const string Referrer = TableNamePrefix + "Referrer";

                /// <summary>
                /// The name of the client error and referrer relation table
                /// </summary>
                public const string ClientError2Referrer = TableNamePrefix + "ClientError2Referrer";

                /// <summary>
                /// The name of the recommendation table
                /// </summary>
                public const string Recommendation = TableNamePrefix + "Recommendation";

                /// <summary>
                /// The name of the redaction score table
                /// </summary>
                public const string RedactionScore = TableNamePrefix + "RedactionScore";
            }

            /// <summary>
            /// Names of columns for aggregate functions
            /// </summary>
            public static class AggregateColumns
            {
                /// <summary>
                /// The name of the column for the total amount of occurrences
                /// </summary>
                public const string TotalOccurrences = "totalOccurrances";

                /// <summary>
                /// The name of the column for the most recent occurrence
                /// </summary>
                public const string MostRecentOccurrence = "mostRecentOccurrence";

                /// <summary>
                /// The name of the column for the most common referrer url
                /// </summary>
                public const string MostCommonReferrer = "mostCommonReferrer";
            }

            /// <summary>
            /// Unique ids for different kinds of client errors
            /// </summary>
            public static class ClientErrorStrategies
            {
                /// <summary>
                /// The unique id for "not found" client errors
                /// </summary>
                public static readonly Guid NotFound = new("69b34892-0c2f-48c5-9739-28b71f1dc675");

                /// <summary>
                /// The unique id for "no longer exists" client errors
                /// </summary>
                public static readonly Guid NoLongerExists = new("a6f75e6e-aa46-4324-9ee1-24d1db26e9a6");
            }

            /// <summary>
            /// Unique ids for different kinds of url redirect matching strategies
            /// </summary>
            public static class RedirectSourceStrategies
            {
                /// <summary>
                /// The unique id for exact url matches
                /// </summary>
                public static readonly Guid Url = new("6406121c-766c-49eb-a510-8d979ea27ff9");

                /// <summary>
                /// The unique id for regular expression matches
                /// </summary>
                public static readonly Guid RegularExpression = new("6dd69e53-b8dc-4a4e-84fb-e76c758bc8a5");
            }

            /// <summary>
            /// Unique ids for different kinds of url redirect strategies
            /// </summary>
            public static class RedirectTargetStrategies
            {
                /// <summary>
                /// The unique id for manual url redirects
                /// </summary>
                public static readonly Guid Url = new("603c97d6-57c8-442f-8631-a8fae383fbf5");

                /// <summary>
                /// The unique id for url redirects to content items
                /// </summary>
                public static readonly Guid Content = new("e9e3a702-54f7-42ae-aadd-5b04185da988");

                /// <summary>
                /// The unique id for url redirects to media items
                /// </summary>
                public static readonly Guid Media = new("376c08bf-c93c-4298-b4b8-61a933c9f99c");
            }

            /// <summary>
            /// Unique ids for the various classifiers for recommendations
            /// </summary>
            public static class RedactionScores
            {
                /// <summary>
                /// Any url without extension
                /// </summary>
                public static readonly Guid Page = new("a23ed85f-d803-4850-baf4-c02203c49b93");

                /// <summary>
                /// Any url with an unrecognised extension
                /// </summary>
                public static readonly Guid File = new("bf6b11a6-196b-4003-ae61-b4b5525f9110");

                /// <summary>
                /// Any url with an image file extension
                /// </summary>
                public static readonly Guid Media = new("b3454454-ee3f-4c99-889f-c570eb096544");

                /// <summary>
                /// Any url with a technical extension (style sheets, scripts, fonts, etc.)
                /// </summary>
                public static readonly Guid TechnicalFile = new("e1de5e1b-c4f4-42ae-a50e-4e0ce3348e62");
            }
        }
    }
}
