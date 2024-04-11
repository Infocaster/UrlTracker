using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;

namespace UrlTracker.Core.Caching.Memory.Notifications
{
    internal class RedirectsCacheRefresher : CacheRefresherBase<RedirectsChangedNotification>
    {
        public static readonly Guid UniqueKey = new("69D966B7-D4B3-447E-A8C3-78C25515DC4A");

        public RedirectsCacheRefresher(
            AppCaches appCaches,
            IEventAggregator eventAggregator,
            ICacheRefresherNotificationFactory factory)
            : base(appCaches, eventAggregator, factory)
        {
        }

        public override Guid RefresherUniqueId => UniqueKey;

        public override string Name => "Redirects change";
    }
}
