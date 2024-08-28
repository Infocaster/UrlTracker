    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UrlTracker.Core;

namespace UrlTracker.IntegrationTests.Backoffice.Recommendation
{
    public abstract class RecommendationTestBase : BackofficeIntegrationTestBase
    {
        protected const string _endpointBase = "/umbraco/backoffice/urltracker/recommendations";

        protected IRedactionScoreService GetRedactionScoreService()
        {
            return ServiceProvider.GetRequiredService<IRedactionScoreService>();
        }

        protected IRecommendationService GetRecommendationService()
        {
            return ServiceProvider.GetRequiredService<IRecommendationService>();
        }
    }
}
