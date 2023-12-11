using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    [ExcludeFromCodeCoverage]
    public class GetSettingsResponse
    {
        public bool IsDisabled { get; set; }
        public bool EnableLogging { get; set; }
        public bool TrackingDisabled { get; set; }
        public bool IsNotFoundTrackingDisabled { get; set; }
        public bool AppendPortNumber { get; set; }
        public List<string> BlockedUrlsList { get; set; } = new List<string>();
        public List<string> AllowedUserAgents { get; set; } = new List<string> { "Safari", "Google Chrome", "Edge", "Mozilla", "Firefox" };
    }
}
