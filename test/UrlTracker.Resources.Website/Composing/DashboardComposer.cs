using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Dashboards;

namespace UrlTracker.Resources.Website.Composing
{
    public class DashboardComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // here to disable all the default dashboards, because it's a test website, we don't need fancy dashboards
            composition.Dashboards().Remove<ContentDashboard>();
            composition.Dashboards().Remove<RedirectUrlDashboard>();
        }
    }
}