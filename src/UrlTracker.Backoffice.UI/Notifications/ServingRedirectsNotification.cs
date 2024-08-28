using System.Collections.Generic;
using Umbraco.Cms.Core.Notifications;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;

namespace UrlTracker.Backoffice.UI.Notifications;

/// <summary>
/// Listen to this notification if you want to apply modifications to the redirect before it is served to the frontend of the backoffice
/// </summary>
public class ServingRedirectsNotification : INotification
{
    internal ServingRedirectsNotification(IEnumerable<RedirectResponse> redirects)
    {
        Redirects = redirects;
    }

    /// <summary>
    /// The redirects that are being served to the frontend
    /// </summary>
    public IEnumerable<RedirectResponse> Redirects { get; }
}
