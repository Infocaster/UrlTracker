using System.Collections.Generic;

namespace UrlTracker.Web.Controllers.Models
{
    public class GetSettingsResponse
    {
        public bool IsDisabled { get; set; }
        public bool EnableLogging { get; set; }
        public bool TrackingDisabled { get; set; }
        public bool IsNotFoundTrackingDisabled { get; set; }
        public bool AppendPortNumber { get; set; }
        public List<string> BlockedUrlsList { get; set; } = new List<string>();
    }
}
