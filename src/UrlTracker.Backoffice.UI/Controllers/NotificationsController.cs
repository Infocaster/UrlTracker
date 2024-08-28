using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Notifications;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [PluginController(Defaults.Routing.Area)]
    [ApiController]
    [Route(Defaults.Routing.Route)]
    internal class NotificationsController : UmbracoAuthorizedApiController
    {
        private readonly INotificationsRequestHandler _requestHandler;

        public NotificationsController(INotificationsRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet("{alias}")]
        [Produces(typeof(IEnumerable<NotificationResponse>))]
        public IActionResult Get([FromRoute] string alias)
        {
            var response = _requestHandler.List(alias);
            return Ok(response);
        }
    }
}
