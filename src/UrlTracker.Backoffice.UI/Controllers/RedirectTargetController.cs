using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [PluginController(Defaults.Routing.Area)]
    [ApiController]
    [Route(Defaults.Routing.Route)]
    internal class RedirectTargetController : UmbracoAuthorizedApiController
    {
        private readonly IRedirectTargetRequestHandler _requestHandler;

        public RedirectTargetController(IRedirectTargetRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet]
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
