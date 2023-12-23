namespace UrlTracker.Core
{
    public static partial class Defaults
    {
        /// <summary>
        /// Constants related to model validation
        /// </summary>
        public static class Validation
        {
            /// <summary>
            /// Use this message when a source url or source regex is required, but neither are specified
            /// </summary>
            public const string SourceConditionNotDefined = "Either SourceUrl or SourceRegex must be specified";

            /// <summary>
            /// Use this message when a target node or target url is required, but neither are specified
            /// </summary>
            public const string TargetConditionNotDefined = "Either TargetNode or TargetUrl must be specified";

            /// <summary>
            /// Use this message when a culture is provided, but in the wrong format
            /// </summary>
            public const string CultureConditionInvalidFormat = "The provided Culture is using an invalid format";

            /// <summary>
            /// Use this message when a regex is provided, but the regex pattern is invalid
            /// </summary>
            public const string RegexConditionInvalidFormat = "The provided Regex is not a valid regex pattern";
        }
    }
}
