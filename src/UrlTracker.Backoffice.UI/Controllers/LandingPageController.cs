using System;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.LandingPage;
using UrlTracker.Core;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [PluginController(Defaults.Routing.Area)]
    [ApiController]
    [Route(Defaults.Routing.Route)]
    internal class LandingPageController : UmbracoAuthorizedApiController
    {
        private readonly IRecommendationService _recommendationService;

        public LandingPageController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet]
        [Produces(typeof(NumericMetricResponse))]
        public IActionResult GetNumericMetric()
        {
            // the numeric metric is all the recommendations that have been updated within the past week
            // It's only about recommendations related to pages, so we include only one type of recommendation
            var now = DateTime.UtcNow;
            var result = _recommendationService.Count(now.AddDays(-7), null, new[]
            {
                Core.Defaults.DatabaseSchema.RedactionScores.Page
            });

            return Ok(new NumericMetricResponse(result));
        }
    }
}
