using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Backoffice.Notifications.Options
{
    /// <summary>
    /// URL Tracker options related to backoffice notifications
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UrlTrackerNotificationsOptions
    {
        /// <summary>
        /// Set this option to <see langword="false" /> to disable handling of content changes
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}
