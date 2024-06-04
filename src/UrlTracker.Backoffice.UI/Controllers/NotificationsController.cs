using System.Collections.Generic;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Backoffice.UI.Controllers.Models.Notifications;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [ApiVersion(Defaults.Routing.V1.ApiVersion)]
    [MapToApi(Defaults.Routing.V1.ApiName)]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [Route(Defaults.Routing.V1.Route)]
    internal class NotificationsController : Controller
    {
        private readonly INotificationsRequestHandler _requestHandler;

        public NotificationsController(INotificationsRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet("{alias}")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(IEnumerable<NotificationResponse>))]
        public IActionResult Get([FromRoute] string alias)
        {
            var response = _requestHandler.List(alias);
            return Ok(response);
        }
    }
}
