using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{

    /// <summary>
    /// Note: these endpoints use POST because HttpPut and HttpDelete are having problems with IIS settings. https://github.com/Infocaster/UrlTracker/issues/76 
    /// </summary>
    [ApiController]
    [PluginController(Defaults.Routing.Area)]
    [Route(Defaults.Routing.Route)]
    internal class RecommendationsController : UmbracoAuthorizedApiController
    {
        private readonly IRecommendationRequestHandler _requestHandler;

        public RecommendationsController(IRecommendationRequestHandler requestHandler)
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

        [HttpPost]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] UpdateRequest request)
        {
            var result = _requestHandler.Update(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult UpdateBulk(IEnumerable<UpdateRequest> request)
        {
            var result = _requestHandler.Update(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Produces(typeof(RecommendationResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromBody] DeleteRequest request)
        {
            var result = _requestHandler.Delete(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

    }
}
