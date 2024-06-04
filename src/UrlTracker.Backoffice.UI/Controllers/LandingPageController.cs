using System;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Backoffice.UI.Controllers.Models.LandingPage;
using UrlTracker.Core;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [ApiVersion(Defaults.Routing.V1.ApiVersion)]
    [MapToApi(Defaults.Routing.V1.ApiName)]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [Route(Defaults.Routing.V1.Route)]
    internal class LandingPageController : Controller
    {
        private readonly IRecommendationService _recommendationService;

        public LandingPageController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("metric")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
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
