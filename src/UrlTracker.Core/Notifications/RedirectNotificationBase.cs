using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Notifications
{
    public class RedirectNotificationBase : INotification
    {
        public RedirectNotificationBase(IReadOnlyCollection<IRedirect> redirects)
        {
            Redirects = redirects;
        }

        public RedirectNotificationBase(IRedirect redirect)
            : this(new List<IRedirect> { redirect })
        {
        }

        /// <summary>
        /// The redirects that are affected by the current operation
        /// </summary>
        public IReadOnlyCollection<IRedirect> Redirects { get; }
    }
}
