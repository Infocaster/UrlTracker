using Umbraco.Cms.Core.Composing;

namespace UrlTracker.Backoffice.UI.Extensions
{
    /// <summary>
    /// The advanced redirects page of the URL Tracker dashboard
    /// </summary>
    [Weight(400)]
    public class AdvancedRedirectsDashboardPage : IUrlTrackerDashboardPage
    {
        /// <inheritdoc/>
        public string Alias { get; } = Defaults.Extensions.AdvancedRedirects;

        /// <inheritdoc/>
        public string View { get; } = Defaults.Routing.DashboardPageFolder + "advancedredirects.html";
    }
}
