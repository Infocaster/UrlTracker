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
            /// Use this message when a source condition is required, but none is specified
            /// </summary>
            public const string SourceConditionNotDefined = "A source condition must be specified";

            /// <summary>
            /// Use this message when a target condition is required, but none is specified
            /// </summary>
            public const string TargetConditionNotDefined = "A target condition must be specified";

            /// <summary>
            /// Use this message when a culture is provided, but in the wrong format
            /// </summary>
            public const string CultureConditionInvalidFormat = "The provided culture is using an invalid format";

            /// <summary>
            /// Use this message when a regex is provided, but the regex pattern is invalid
            /// </summary>
            public const string RegexConditionInvalidFormat = "The provided Regex is not a valid regex pattern";
        }
    }
}
