using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{

    /// <summary>
    /// Note: these endpoints use POST because HttpPut and HttpDelete are having problems with IIS settings. https://github.com/Infocaster/UrlTracker/issues/76 
    /// </summary>
    [ApiController]
    [VersionedApiBackOfficeRoute(Defaults.Routing.Area + "/Recommendations")]
    [ApiExplorerSettings(GroupName = "Recommendations")]
    [MapToApi(Defaults.Routing.SwaggerApi)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    internal class UrlTrackerRecommendationsController : ManagementApiControllerBase
    {
        private readonly IRecommendationRequestHandler _requestHandler;

        public UrlTrackerRecommendationsController(IRecommendationRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet]
        [Produces(typeof(RecommendationCollectionResponse))]
        public IActionResult List([FromQuery] ListRecommendationRequest request)
        {
            var result = _requestHandler.Get(request);
            return Ok(result);
        }

        [HttpPost("{recommendationId}")]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Update([FromRoute] int recommendationId, [FromBody] UpdateRecommendationRequest request)
        {
            var result = _requestHandler.Update(recommendationId, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("updatebulk")]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult UpdateBulk(IEnumerable<EntityWithIdRequest<UpdateRecommendationRequest>> request)
        {
            var result = _requestHandler.Update(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{recommendationId}/delete")]
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
