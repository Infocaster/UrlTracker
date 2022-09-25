namespace UrlTracker.Backoffice.UI
{
    public static partial class Defaults
    {
        /// <inheritdoc cref="Core.Defaults.Validation" />
        public static class Validation
        {
            /// <inheritdoc cref="Core.Defaults.Validation.SourceConditionNotDefined" />
            public const string SourceConditionNotDefined = Core.Defaults.Validation.SourceConditionNotDefined;

            /// <inheritdoc cref="Core.Defaults.Validation.TargetConditionNotDefined" />
            public const string TargetConditionNotDefined = Core.Defaults.Validation.TargetConditionNotDefined;

            /// <summary>
            /// Use this message when a root node id is required, but is not specified
            /// </summary>
            public const string RedirectRootNodeIdNotSpecified = "The field RedirectRootNodeId is required";
        }
    }
}
