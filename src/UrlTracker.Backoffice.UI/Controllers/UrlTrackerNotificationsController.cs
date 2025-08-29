using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using UrlTracker.Backoffice.UI.Controllers.Models.Notifications;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [VersionedApiBackOfficeRoute(Defaults.Routing.Area + "/Notifications")]
    [ApiExplorerSettings(GroupName = "Notifications")]
    [MapToApi(Defaults.Routing.SwaggerApi)]
    internal class UrlTrackerNotificationsController : ManagementApiControllerBase
    {
        private readonly INotificationsRequestHandler _requestHandler;

        public UrlTrackerNotificationsController(INotificationsRequestHandler requestHandler)
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
