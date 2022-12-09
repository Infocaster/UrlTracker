using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core
{
    public interface IRecommendationService
    {
        IRecommendation Create(string url, IRedactionScore score);
        IRecommendation Create(string url, Guid scoreKey);
        RecommendationEntityCollection Get(uint page, uint pageSize);
        void Save(IRecommendation recommendation);
    }

    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IScopeProvider _scopeProvider;
        private readonly IRedactionScoreService _redactionScoreService;

        public RecommendationService(IRecommendationRepository recommendationRepository, IScopeProvider scopeProvider, IRedactionScoreService redactionScoreService)
        {
            _recommendationRepository = recommendationRepository;
            _scopeProvider = scopeProvider;
            _redactionScoreService = redactionScoreService;
        }

        public RecommendationEntityCollection Get(uint page, uint pageSize)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var result = _recommendationRepository.Get(page, pageSize, Core.Defaults.Parameters.ScoreParameters);

            return result;
        }

        public void Save(IRecommendation recommendation)
        {
            using var scope = _scopeProvider.CreateScope();

            _recommendationRepository.Save(recommendation);

            scope.Complete();
        }

        public IRecommendation Create(string url, Guid scoreKey)
        {
            var score = _redactionScoreService.Get(scoreKey);

            if (score is null) throw new ArgumentException("No redaction score exists for given key", nameof(scoreKey));

            return Create(url, score);
        }

        public IRecommendation Create(string url, IRedactionScore score)
        {
            return new RecommendationEntity(url, score);
        }
    }
}
