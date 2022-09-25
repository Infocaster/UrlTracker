namespace UrlTracker.Middleware.Options
{
    /// <summary>
    /// URL Tracker options related to middleware
    /// </summary>
    public class UrlTrackerPipelineOptions
    {
        /// <summary>
        /// Set this value to false to disable all middleware
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// Set this value to false to disable client error tracking
        /// </summary>
        public bool EnableClientErrorTracking { get; set; } = true;
    }
}
