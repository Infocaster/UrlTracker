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
    }
}
