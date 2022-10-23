using Umbraco.Cms.Core.Composing;

namespace UrlTracker.Backoffice.UI.Extensions
{
    /// <summary>
    /// The frontpage overview of the URL Tracker
    /// </summary>
    [Weight(100)]
    public class OverviewDashboardPage : IUrlTrackerDashboardPage
    {
        /// <inheritdoc />
        public string Alias { get; } = "overview";

        /// <inheritdoc />
        public string View { get; } = Defaults.Routing.DashboardPageFolder + "overview.html";
    }
}
