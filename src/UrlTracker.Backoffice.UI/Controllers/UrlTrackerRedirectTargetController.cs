using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [VersionedApiBackOfficeRoute(Defaults.Routing.Area + "/RedirectTarget")]
    [ApiExplorerSettings(GroupName = "Redirects")]
    [MapToApi(Defaults.Routing.SwaggerApi)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    internal class UrlTrackerRedirectTargetController : ManagementApiControllerBase
    {
        private readonly IRedirectTargetRequestHandler _requestHandler;

        public UrlTrackerRedirectTargetController(IRedirectTargetRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet("content")]
        [Produces(typeof(ContentTargetResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Content([FromQuery] GetContentTargetRequest request)
        {
            var model = _requestHandler.GetContentTarget(request);
            if (model is null) return NotFound();

            return Ok(model);
        }
    }
}
