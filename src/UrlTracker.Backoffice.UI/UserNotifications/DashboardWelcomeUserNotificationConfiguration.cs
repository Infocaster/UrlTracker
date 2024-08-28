using System;
using Microsoft.Extensions.Options;

namespace UrlTracker.Backoffice.UI.UserNotifications
{
    internal class DashboardWelcomeUserNotificationConfiguration
        : IConfigureNamedOptions<UrlTrackerUserNotificationOptions>
    {
        public void Configure(string name, UrlTrackerUserNotificationOptions options)
        {
            options.Notifications.Add(new UrlTrackerUserNotification(
                "0a224b9b-" + name,
                $"urlTrackerNotifications_{name}welcometitle",
                Array.Empty<string>(),
                $"urlTrackerNotifications_{name}welcomebody",
                Array.Empty<string>()));
        }

        // Nothing to configure for unnamed options
        public void Configure(UrlTrackerUserNotificationOptions options)
        { }
    }
}
