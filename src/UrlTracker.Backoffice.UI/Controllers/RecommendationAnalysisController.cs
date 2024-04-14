using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations.Analysis;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.UI.Controllers;

[ApiController]
[PluginController(Defaults.Routing.Area)]
[Route(Defaults.Routing.Route)]
internal class RecommendationAnalysisController : UmbracoAuthorizedApiController
{
    private readonly IRecommendationAnalysisRequestHandler _requestHandler;

    public RecommendationAnalysisController(IRecommendationAnalysisRequestHandler requestHandler)
    {
        _requestHandler = requestHandler;
    }

    [HttpGet]
    [Route("{id}")]
    [Produces(typeof(RecommendationHistory))]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistoryAsync([FromRoute] int id, [FromQuery] int pastDays = 20)
    {
        var result = await _requestHandler.GetHistoryAsync(new RecommendationHistoryRequest()
        {
            Id = id,
            PastDays = pastDays
        });

        if (result is null) return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    [Produces(typeof(IEnumerable<ReferrerResponse>))]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReferrersAsync([FromRoute] int id)
    {
        var result = await _requestHandler.GetMostCommonReferrersAsync(id);
        if (result == null) return NotFound();

        return Ok(result);
    }
}
