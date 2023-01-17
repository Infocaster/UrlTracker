using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.UserNotifications;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [PluginController(Defaults.Routing.Area)]
    internal class NotificationsController : UmbracoAuthorizedApiController
    {
        private readonly IOptionsSnapshot<UrlTrackerUserNotificationOptions> _notificationOptions;

        public NotificationsController(IOptionsSnapshot<UrlTrackerUserNotificationOptions> notificationOptions)
        {
            _notificationOptions = notificationOptions;
        }

        [HttpGet]
        public IActionResult Get(string alias)
        {
            var model = _notificationOptions.Get(alias);
            return Ok(model);
        }
    }
}
