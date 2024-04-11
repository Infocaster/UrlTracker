using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Sync;

namespace UrlTracker.Core.Caching.Memory.Notifications
{
    /// <summary>
    /// This notification is fired when the collection of redirects changes and caches should be re-evaluated
    /// </summary>
    public class RedirectsChangedNotification
        : CacheRefresherNotification
    {
        /// <inheritdoc />
        public RedirectsChangedNotification(object messageObject, MessageType messageType)
            : base(messageObject, messageType)
        {
        }
    }
}
