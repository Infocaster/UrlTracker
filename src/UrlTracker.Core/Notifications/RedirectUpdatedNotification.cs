using System.Collections.Generic;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Notifications
{
    public class RedirectUpdatedNotification
        : RedirectNotificationBase
    {
        public RedirectUpdatedNotification(IReadOnlyCollection<IRedirect> redirects)
            : base(redirects)
        {
        }

        public RedirectUpdatedNotification(IRedirect redirect)
            : base(redirect)
        {
        }
    }
}
