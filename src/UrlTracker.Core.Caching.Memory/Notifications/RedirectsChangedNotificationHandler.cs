using Microsoft.Extensions.Caching.Memory;
using Umbraco.Cms.Core.Events;

namespace UrlTracker.Core.Caching.Memory.Notifications
{
    internal class RedirectsChangedNotificationHandler
        : INotificationHandler<RedirectsChangedNotification>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IInterceptCache _interceptCache;

        public RedirectsChangedNotificationHandler(
            IMemoryCache memoryCache,
            IInterceptCache interceptCache)
        {
            _memoryCache = memoryCache;
            _interceptCache = interceptCache;
        }

        public void Handle(RedirectsChangedNotification notification)
        {
            _memoryCache.Remove(Defaults.Cache.RegexRedirectKey);
            _interceptCache.Clear();
        }
    }
}
