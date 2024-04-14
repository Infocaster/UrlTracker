using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Scoring;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [PluginController(Defaults.Routing.Area)]
    [Route(Defaults.Routing.Route)]
    internal class ScoringController
        : UmbracoAuthorizedApiController
    {
        private readonly IScoringRequestHandler _requestHandler;

        public ScoringController(IScoringRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet]
        [Produces(typeof(IEnumerable<RedactionScoreResponse>))]
        public IActionResult RedactionScores()
        {
            var result = _requestHandler.ListRedactionScores();
            return Ok(result);
        }

        [HttpGet]
        [Produces(typeof(ScoreParametersResponse))]
        public IActionResult ScoreParameters()
        {
            var result = _requestHandler.GetScoreParameters();
            return Ok(result);
        }
    }
}
