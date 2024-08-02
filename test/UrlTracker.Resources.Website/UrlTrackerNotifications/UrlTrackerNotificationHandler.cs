using Umbraco.Cms.Core.Events;
using UrlTracker.Core.Notifications;

namespace UrlTracker.Resources.Website.UrlTrackerNotifications
{
    public class UrlTrackerNotificationHandler
        : INotificationHandler<RedirectCreatedNotification>
        , INotificationHandler<RedirectUpdatedNotification>
        , INotificationHandler<RedirectDeletedNotification>
    {
        public void Handle(RedirectCreatedNotification notification)
        {
        }

        public void Handle(RedirectUpdatedNotification notification)
        {
        }

        public void Handle(RedirectDeletedNotification notification)
        {
        }
    }
}
