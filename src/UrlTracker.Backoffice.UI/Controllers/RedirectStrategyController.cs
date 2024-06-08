using System.Collections;
using System.Collections.Generic;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectStrategy;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [ApiVersion(Defaults.Routing.V1.ApiVersion)]
    [MapToApi(Defaults.Routing.V1.ApiName)]
    [ApiExplorerSettings(GroupName = "Redirect target")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [Route(Defaults.Routing.V1.Route)]
    internal class RedirectStrategyController : Controller
    {
        private readonly IRedirectStrategyRequestHandler _requestHandler;

        public RedirectStrategyController(IRedirectStrategyRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet("content")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(ContentTargetResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Content([FromQuery] GetContentTargetRequest request)
        {
            var model = _requestHandler.GetContentTarget(request);
            if (model is null) return NotFound();

            return Ok(model);
        }

        [HttpGet("sources")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(IEnumerable<RedirectStrategyResponse>))]
        public IActionResult Sources()
        {
            var model = _requestHandler.GetSources();
            return Ok(model);
        }

        [HttpGet("targets")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(IEnumerable<RedirectStrategyResponse>))]
        public IActionResult Targets()
        {
            var model = _requestHandler.GetTargets();
            return Ok(model);
        }
    }
}
