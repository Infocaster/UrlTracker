using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Configuration.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerSettings
    {
        public bool IsDisabled { get; set; } = false;
        public bool IsTrackingDisabled { get; set; } = false;
        public bool LoggingEnabled { get; set; } = true;
        public bool IsNotFoundTrackingDisabled { get; set; } = false;
        public bool AppendPortNumber { get; set; } = false;
        public bool HasDomainOnChildNode { get; set; } = false;
        public bool CacheRegexRedirects { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Value must be 1 or more")]
        public int? InterceptSlidingCacheMinutes { get; set; } = 60 * 24 * 2; // cache for 2 days by default
        public bool EnableInterceptCaching { get; set; } = true;
        public long MaxCachedIntercepts { get; set; } = 5000;
        public List<string> BlockedUrlsList { get; set; } = new List<string>();
    }
}
