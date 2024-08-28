using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using UrlTracker.Backoffice.UI.Controllers.Models.Notifications;
using UrlTracker.Backoffice.UI.UserNotifications;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface INotificationsRequestHandler
    {
        IEnumerable<NotificationResponse> List(string alias);
    }

    internal class NotificationsRequestHandler : INotificationsRequestHandler
    {
        private readonly IOptionsSnapshot<UrlTrackerUserNotificationOptions> _notificationOptions;

        public NotificationsRequestHandler(IOptionsSnapshot<UrlTrackerUserNotificationOptions> notificationOptions)
        {
            _notificationOptions = notificationOptions;
        }

        public IEnumerable<NotificationResponse> List(string alias)
        {
            var model = _notificationOptions.Get(alias);
            return model.Notifications.Select(n => new NotificationResponse(
                n.Id,
                n.TranslatableTitleComponent,
                n.TitleArguments,
                n.TranslatableBodyComponent,
                n.BodyArguments));
        }
    }
}
