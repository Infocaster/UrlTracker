using System;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;

internal class UpdateRecommendationRequest
{
    public Guid? RecommendationStrategy { get; set; }

    public bool? Ignore { get; set; }

}