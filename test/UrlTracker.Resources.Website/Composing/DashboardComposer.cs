using System;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;

namespace UrlTracker.Resources.Website.Composing
{
    public class DashboardComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // here to disable all the default dashboards, because it's a test website, we don't need fancy dashboards
            //builder.Dashboards()!.Remove<ContentDashboard>();
            builder.Dashboards()!.Remove<RedirectUrlDashboard>();
            
            // Add a dashboard for experimenting with the settings
            builder.Dashboards()!.Add<UrlTrackerTestDashboard>();
        }
    }

    [Weight(1000)]
    public class UrlTrackerTestDashboard : IDashboard
    {
        public string[] Sections { get; } = new[] { Constants.Applications.Content };

        public IAccessRule[] AccessRules { get; } = Array.Empty<IAccessRule>();

        public string? Alias { get; } = "UrlTrackerTest";

        public string? View => "/App_Plugins/UrlTrackerTest/dashboard/dashboard.html";
    }
}