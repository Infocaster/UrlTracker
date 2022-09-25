using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace UrlTracker.Core.Caching.Memory.Notifications
{
    /// <summary>
    /// A notification handler that empties the cached domains when umbraco domains change
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContentChangeNotificationHandler
        : INotificationHandler<DomainDeletedNotification>, INotificationHandler<DomainSavedNotification>
    {
        private readonly IAppPolicyCache _runtimeCache;

        /// <inheritdoc />
        public ContentChangeNotificationHandler(AppCaches appCaches)
        {
            _runtimeCache = appCaches.RuntimeCache;
        }

        void INotificationHandler<DomainDeletedNotification>.Handle(DomainDeletedNotification notification)
        {
            _runtimeCache.Clear(Defaults.Cache.DomainKey);
        }

        void INotificationHandler<DomainSavedNotification>.Handle(DomainSavedNotification notification)
        {
            _runtimeCache.Clear(Defaults.Cache.DomainKey);
        }
    }
}
