using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Configuration.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerSettings
    {
        public bool Enable { get; set; } = true;
        public bool EnableLogging { get; set; } = true;
        public List<string> BlockedUrlsList { get; set; } = new List<string>();
        public List<string> AllowedUserAgents { get; set; } = new List<string> { "Safari", "Google Chrome", "Edge", "Mozilla", "Firefox" };
    }
}
