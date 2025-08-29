using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using UrlTracker.Backoffice.UI.Controllers.Models.Scoring;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [VersionedApiBackOfficeRoute(Defaults.Routing.Area + "/Scoring")]
    [ApiExplorerSettings(GroupName = "Recommendations")]
    [MapToApi(Defaults.Routing.SwaggerApi)]
    internal class UrlTrackerScoringController
        : ManagementApiControllerBase
    {
        private readonly IScoringRequestHandler _requestHandler;

        public UrlTrackerScoringController(IScoringRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet("redactionscores")]
        [Produces(typeof(IEnumerable<RedactionScoreResponse>))]
        public IActionResult RedactionScores()
        {
            var result = _requestHandler.ListRedactionScores();
            return Ok(result);
        }

        [HttpGet("scoreparameters")]
        [Produces(typeof(ScoreParametersResponse))]
        public IActionResult ScoreParameters()
        {
            var result = _requestHandler.GetScoreParameters();
            return Ok(result);
        }
    }
}
