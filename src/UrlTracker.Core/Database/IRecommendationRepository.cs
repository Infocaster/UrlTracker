using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database
{
    public interface IRecommendationRepository
        : IReadWriteQueryRepository<int, IRecommendation>
    {
        RecommendationEntityCollection Get(int page, int pageSize);
    }
}
