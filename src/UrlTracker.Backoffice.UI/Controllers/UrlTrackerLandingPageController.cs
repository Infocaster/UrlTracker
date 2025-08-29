using System;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using UrlTracker.Backoffice.UI.Controllers.Models.LandingPage;
using UrlTracker.Core;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [VersionedApiBackOfficeRoute(Defaults.Routing.Area + "/LandingPage")]
    [ApiExplorerSettings(GroupName = "Landing page")]
    [MapToApi(Defaults.Routing.SwaggerApi)]
    internal class UrlTrackerLandingPageController : ManagementApiControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public UrlTrackerLandingPageController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("metric")]
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
