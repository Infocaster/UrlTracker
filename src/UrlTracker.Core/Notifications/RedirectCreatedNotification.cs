using System.Collections.Generic;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Notifications
{
    public class RedirectCreatedNotification
        : RedirectNotificationBase
    {
        public RedirectCreatedNotification(IReadOnlyCollection<IRedirect> redirects)
            : base(redirects)
        {
        }

        public RedirectCreatedNotification(IRedirect redirect)
            : base(redirect)
        {
        }
    }
}
