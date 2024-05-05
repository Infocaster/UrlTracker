using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UrlTracker.Backoffice.Notifications;
using UrlTracker.Backoffice.UI;
using UrlTracker.Core;
using UrlTracker.Core.Caching.Memory;
using UrlTracker.Middleware;
using UrlTracker.Modules.Options;
using UrlTracker.Web;

namespace UrlTracker
{
    /// <summary>
    /// A composer that registers all services for a standard URL Tracker installation
    /// </summary>
    public class UrlTrackerComposer : IComposer
    {
        /// <inheritdoc />
        public void Compose(IUmbracoBuilder builder)
        {
            builder
                .ComposeUrlTrackerCore()
                .ComposeUrlTrackerWeb()
                .ComposeUrlTrackerMemoryCache()
                .ComposeUrlTrackerBackoffice()
                .ComposeUrlTrackerBackofficeNotifications()
                .ComposeUrlTrackerMiddleware();

            builder.Services.AddUrlTrackerModule("Standard installation");
        }
    }
}
