using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Sync;
using UrlTracker.Core.Caching.Memory.Notifications;
using UrlTracker.Core.Caching.Memory.Options;

namespace UrlTracker.Core.Caching.Memory.Active
{
    internal class ActiveCacheRedirectChangeHandler
        : INotificationHandler<RedirectsChangedNotification>
    {
        private readonly IActiveRedirectCacheWriter _cacheWriter;
        private readonly IOptions<UrlTrackerMemoryCacheOptions> _options;
        private readonly Logging.ILogger<ActiveCacheRedirectChangeHandler> _logger;

        public ActiveCacheRedirectChangeHandler(
            IActiveRedirectCacheWriter cacheWriter,
            IOptions<UrlTrackerMemoryCacheOptions> options,
            Logging.ILogger<ActiveCacheRedirectChangeHandler> logger)
        {
            _cacheWriter = cacheWriter;
            _options = options;
            _logger = logger;
        }

        public void Handle(RedirectsChangedNotification notification)
        {
            if (!_options.Value.EnableActiveCache) return;

            switch (notification.MessageType)
            {
                case MessageType.RefreshById:
                    _cacheWriter.RefreshRedirect(Convert.ToInt32(notification.MessageObject));
                    break;
                case MessageType.RefreshAll:
                    _cacheWriter.RefreshRedirects();
                    break;
                case MessageType.RemoveById:
                    _cacheWriter.RemoveRedirect(Convert.ToInt32(notification.MessageObject));
                    break;
                default:
                    _logger.LogWarning("Message type {messageType} for Redirect cache refresher was unexpected", notification.MessageType);
                    break;
            }
        }
    }
}
