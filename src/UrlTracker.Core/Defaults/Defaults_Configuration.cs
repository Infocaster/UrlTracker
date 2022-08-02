namespace UrlTracker.Core
{
    public static partial class Defaults
    {
        public static class Configuration
        {
            public const string Prefix = "urlTracker:";
            public const string Disabled = "disabled";
            public const string TrackingDisabled = "trackingDisabled";
            public const string NotFoundTrackingDisabled = "notFoundTrackingDisabled";
            public const string EnableLogging = "enableLogging";
            public const string AppendPortNumber = "appendPortNumber";
            public const string HasDomainOnChildNode = "hasDomainOnChildNode";
            public const string MaxCachedIntercepts = "maxCachedIntercepts";
            public const string CacheRegexRedirects = "cacheRegexRedirects";
            public const string InterceptSlidingCacheMinutes = "interceptSlidingCacheMinutes";
            public const string EnableInterceptCaching = "enableInterceptCaching";
            public const string BlockedUrlsList = "blockedUrlsList";
        }
    }
}
