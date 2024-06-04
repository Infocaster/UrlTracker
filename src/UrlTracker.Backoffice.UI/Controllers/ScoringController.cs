using System.Collections.Generic;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Backoffice.UI.Controllers.Models.Scoring;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [ApiVersion(Defaults.Routing.V1.ApiVersion)]
    [MapToApi(Defaults.Routing.V1.ApiName)]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [Route(Defaults.Routing.V1.Route)]
    internal class ScoringController
        : Controller
    {
        private readonly IScoringRequestHandler _requestHandler;

        public ScoringController(IScoringRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet("redactionscores")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(IEnumerable<RedactionScoreResponse>))]
        public IActionResult RedactionScores()
        {
            var result = _requestHandler.ListRedactionScores();
            return Ok(result);
        }

        [HttpGet("scoreparameters")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(ScoreParametersResponse))]
        public IActionResult ScoreParameters()
        {
            var result = _requestHandler.GetScoreParameters();
            return Ok(result);
        }
    }
}
