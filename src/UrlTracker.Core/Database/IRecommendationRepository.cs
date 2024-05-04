using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    public interface IRecommendationRepository
        : IReadWriteQueryRepository<int, IRecommendation>
    {
        Task CleanupAsync(double upperScore, RecommendationScoreParameters parameters);
        void Clear();
        RecommendationEntityCollection Get(uint page, uint pageSize, RecommendationScoreParameters parameters, RecommendationOrderingOptions orderingOptions, RecommendationFilterOptions filterOptions);
    }
}
