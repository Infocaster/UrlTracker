using Umbraco.Cms.Core.Composing;

namespace UrlTracker.Backoffice.UI.Extensions
{
    /// <summary>
    /// The redirects page on the URL Tracker dashboard
    /// </summary>
    [Weight(300)]
    public class RedirectsDashboardPage : IUrlTrackerDashboardPage
    {
        /// <inheritdoc/>
        public string Alias { get; } = Defaults.Extensions.Redirects;

        /// <inheritdoc/>
        public string View { get; } = Defaults.Routing.DashboardPageFolder + "redirects.html";
    }
}
