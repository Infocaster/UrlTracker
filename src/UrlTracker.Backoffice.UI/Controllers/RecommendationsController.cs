using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [PluginController(Defaults.Routing.Area)]
    internal class RecommendationsController : UmbracoAuthorizedApiController
    {
        private readonly IRecommendationRequestHandler _requestHandler;

        public RecommendationsController(IRecommendationRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet]
        public IActionResult List([FromQuery] ListRecommendationRequest request)
        {
            var result = _requestHandler.Get(request);

            return Ok(result);
        }
    }
}
