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
        }
    }
}
