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
            typeof(RedirectTargetController),
            typeof(RedirectsController),
            typeof(RecommendationsController),
            typeof(RecommendationAnalysisController),
            typeof(NotificationsController),
            typeof(RedirectImportController),
            typeof(LandingPageController),
            typeof(ScoringController)
        };

        protected override bool IsController(TypeInfo typeInfo)
        {
            return _controllers.Any(c => c.IsAssignableTo(typeInfo));
        }
    }
}
