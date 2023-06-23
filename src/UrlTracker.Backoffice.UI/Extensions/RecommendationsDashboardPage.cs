using Umbraco.Cms.Core.Composing;

namespace UrlTracker.Backoffice.UI.Extensions
{
    /// <summary>
    /// The recommendations page on the URL Tracker dashboard
    /// </summary>
    [Weight(200)]
    public class RecommendationsDashboardPage : IUrlTrackerDashboardPage
    {
        /// <inheritdoc />
        public string Alias { get; } = Defaults.Extensions.Recommendations;

        /// <inheritdoc />
        public string View { get; } = "urltracker-recommendations-tab";
    }
}
