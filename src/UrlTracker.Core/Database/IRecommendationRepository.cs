using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    public interface IRecommendationRepository
        : IReadWriteQueryRepository<int, IRecommendation>
    {
        RecommendationEntityCollection Get(uint page, uint pageSize, RecommendationScoreParameters parameters);
    }
}
