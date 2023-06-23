﻿using Umbraco.Cms.Core.Composing;

namespace UrlTracker.Backoffice.UI.Extensions
{
    /// <summary>
    /// The frontpage overview of the URL Tracker
    /// </summary>
    [Weight(100)]
    public class OverviewDashboardPage : IUrlTrackerDashboardPage
    {
        /// <inheritdoc />
        public string Alias { get; } = Defaults.Extensions.Overview;

        /// <inheritdoc />
        public string View { get; } = "urltracker-landing-tab";
    }
}
