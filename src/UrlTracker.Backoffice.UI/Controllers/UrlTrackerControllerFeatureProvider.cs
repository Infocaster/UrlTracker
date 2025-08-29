using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    internal class UrlTrackerControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly static Type[] _controllers = new[]
        {
            typeof(UrlTrackerRedirectTargetController),
            typeof(UrlTrackerRedirectsController),
            typeof(UrlTrackerRecommendationsController),
            typeof(UrlTrackerRecommendationAnalysisController),
            typeof(UrlTrackerNotificationsController),
            typeof(UrlTrackerRedirectImportController),
            typeof(UrlTrackerLandingPageController),
            typeof(UrlTrackerScoringController)
        };

        protected override bool IsController(TypeInfo typeInfo)
        {
            return _controllers.Any(c => c.IsAssignableTo(typeInfo));
        }
    }
}
