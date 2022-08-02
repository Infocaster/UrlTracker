using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Configuration.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerSettings
    {
        public UrlTrackerSettings(bool isDisabled, bool isTrackingDisabled, bool loggingEnabled, bool isNotFoundTrackingDisabled, bool appendPortNumber, bool hasDomainOnChildNode, long maxCachedIntercepts, bool cacheRegexRedirects, int? interceptSlidingCacheMinutes, bool enableInterceptCaching, List<string> blockedUrlsList)
        {
            IsDisabled = isDisabled;
            IsTrackingDisabled = isTrackingDisabled;
            LoggingEnabled = loggingEnabled;
            IsNotFoundTrackingDisabled = isNotFoundTrackingDisabled;
            AppendPortNumber = appendPortNumber;
            HasDomainOnChildNode = hasDomainOnChildNode;
            MaxCachedIntercepts = maxCachedIntercepts;
            CacheRegexRedirects = cacheRegexRedirects;
            InterceptSlidingCacheMinutes = interceptSlidingCacheMinutes;
            EnableInterceptCaching = enableInterceptCaching;
            BlockedUrlsList = blockedUrlsList;
        }

        public bool IsDisabled { get; }
        public bool IsTrackingDisabled { get; }
        public bool LoggingEnabled { get; }
        public bool IsNotFoundTrackingDisabled { get; }
        public bool AppendPortNumber { get; }
        public bool HasDomainOnChildNode { get; }
        public long MaxCachedIntercepts { get; }
        public bool CacheRegexRedirects { get; }
        public int? InterceptSlidingCacheMinutes { get; }
        public bool EnableInterceptCaching { get; }
        public List<string> BlockedUrlsList { get; set; } = new List<string>();
    }
}
