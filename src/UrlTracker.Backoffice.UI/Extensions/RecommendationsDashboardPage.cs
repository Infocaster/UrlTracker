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
        public string Alias { get; } = "recommendations";

        /// <inheritdoc />
        public string View { get; } = Defaults.Routing.DashboardPageFolder + "recommendations.html";
    }
}
