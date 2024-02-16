using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using UrlTracker.Core.Caching.Memory.Notifications;
using UrlTracker.Core.Caching.Memory.Options;

namespace UrlTracker.Core.Caching.Memory.Active
{
    internal class ActiveCacheRedirectChangeHandler
        : INotificationHandler<RedirectsChangedNotification>
    {
        private readonly IActiveRedirectCacheWriter _cacheWriter;
        private readonly IOptions<UrlTrackerMemoryCacheOptions> _options;

        public ActiveCacheRedirectChangeHandler(
            IActiveRedirectCacheWriter cacheWriter,
            IOptions<UrlTrackerMemoryCacheOptions> options)
        {
            _cacheWriter = cacheWriter;
            _options = options;
        }

        public void Handle(RedirectsChangedNotification notification)
        {
            if (!_options.Value.EnableActiveCache) return;

            _cacheWriter.RefreshRedirects();
        }
    }
}
