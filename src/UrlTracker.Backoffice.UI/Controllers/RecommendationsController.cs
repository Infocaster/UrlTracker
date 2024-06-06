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
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{

    /// <summary>
    /// Note: these endpoints use POST because HttpPut and HttpDelete are having problems with IIS settings. https://github.com/Infocaster/UrlTracker/issues/76 
    /// </summary>
    [ApiController]
    [ApiVersion(Defaults.Routing.V1.ApiVersion)]
    [MapToApi(Defaults.Routing.V1.ApiName)]
    [ApiExplorerSettings(GroupName = "Recommendations")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [Route(Defaults.Routing.V1.Route)]
    internal class RecommendationsController : Controller
    {
        private readonly IRecommendationRequestHandler _requestHandler;

        public RecommendationsController(IRecommendationRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(RecommendationCollectionResponse))]
        public IActionResult List([FromQuery] ListRecommendationRequest request)
        {
            var result = _requestHandler.Get(request);
            return Ok(result);
        }

        [HttpPost("{recommendationId}")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Update([FromRoute] int recommendationId, [FromBody] UpdateRecommendationRequest request)
        {
            var result = _requestHandler.Update(recommendationId, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("updatebulk")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult UpdateBulk(IEnumerable<EntityWithIdRequest<UpdateRecommendationRequest>> request)
        {
            var result = _requestHandler.Update(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{recommendationId}/delete")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int recommendationId)
        {
            var result = _requestHandler.Delete(recommendationId);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
