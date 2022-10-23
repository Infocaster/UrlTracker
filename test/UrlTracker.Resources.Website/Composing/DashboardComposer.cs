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
        }
    }
}