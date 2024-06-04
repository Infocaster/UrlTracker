using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations.Analysis;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.UI.Controllers;

[ApiController]
[ApiVersion(Defaults.Routing.V1.ApiVersion)]
[MapToApi(Defaults.Routing.V1.ApiName)]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
[Route(Defaults.Routing.V1.Route)]
internal class RecommendationAnalysisController : Controller
{
    private readonly IRecommendationAnalysisRequestHandler _requestHandler;

    public RecommendationAnalysisController(IRecommendationAnalysisRequestHandler requestHandler)
    {
        _requestHandler = requestHandler;
    }

    [HttpGet("{recommendationId}/history")]
    [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
    [Produces(typeof(RecommendationHistory))]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistoryAsync([FromRoute] int recommendationId, [FromQuery] int pastDays = 20)
    {
        var result = await _requestHandler.GetHistoryAsync(new RecommendationHistoryRequest()
        {
            Id = recommendationId,
            PastDays = pastDays
        });

        if (result is null) return NotFound();

        return Ok(result);
    }

    [HttpGet("{recommendationId}/referrers")]
    [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
    [Produces(typeof(IEnumerable<ReferrerResponse>))]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReferrersAsync([FromRoute] int recommendationId)
    {
        var result = await _requestHandler.GetMostCommonReferrersAsync(recommendationId);
        if (result == null) return NotFound();

        return Ok(result);
    }
}
