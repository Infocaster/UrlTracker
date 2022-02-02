namespace UrlTracker.Web.Controllers.Models
{
    public class GetSettingsResponse
    {
        public bool IsDisabled { get; set; }
        public bool EnableLogging { get; set; }
        public bool TrackingDisabled { get; set; }
        public bool IsNotFoundTrackingDisabled { get; set; }
        public bool AppendPortNumber { get; set; }
    }
}
