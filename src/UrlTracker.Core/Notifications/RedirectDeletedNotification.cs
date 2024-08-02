using System.Collections.Generic;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Notifications
{
    public class RedirectDeletedNotification
        : RedirectNotificationBase
    {
        public RedirectDeletedNotification(IReadOnlyCollection<IRedirect> redirects)
            : base(redirects)
        {
        }

        public RedirectDeletedNotification(IRedirect redirect)
            : base(redirect)
        {
        }
    }
}
