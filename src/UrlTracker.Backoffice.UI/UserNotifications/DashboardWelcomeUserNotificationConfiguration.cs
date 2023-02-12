using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace UrlTracker.Backoffice.UI.UserNotifications
{
    internal class DashboardWelcomeUserNotificationConfiguration
        : IConfigureNamedOptions<UrlTrackerUserNotificationOptions>
    {
        public void Configure(string name, UrlTrackerUserNotificationOptions options)
        {
            options.Notifications.Add(new UrlTrackerUserNotification
            {
                Id = "0a224b9b-" + name,
                TranslatableTitleComponent = $"urlTrackerNotifications_{name}welcometitle",
                TitleArguments = Array.Empty<string>(),
                TranslatableBodyComponent = $"urlTrackerNotifications_{name}welcomebody",
                BodyArguments = Array.Empty<string>(),
            });
        }

        // Nothing to configure for unnamed options
        public void Configure(UrlTrackerUserNotificationOptions options)
        { }
    }
}
