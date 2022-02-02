namespace UrlTracker.Core
{
    public static partial class Defaults
    {
        public static class Validation
        {
            public const string SourceConditionNotDefined = "Either SourceUrl or SourceRegex must be specified";
            public const string TargetConditionNotDefined = "Either TargetNode or TargetUrl must be specified";
            public const string QueryParamsMissing = "Query parameters are mandatory";
            public const string BodyContentMissing = "Body content is mandatory";
            public const string RedirectRootNodeIdNotSpecified = "The field RedirectRootNodeId is required";
        }
    }
}
