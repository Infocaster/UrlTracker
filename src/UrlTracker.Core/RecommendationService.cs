using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core
{
    public interface IRecommendationService
    {
        void Clear();
        IRecommendation Create(string url, IRedactionScore score);
        IRecommendation Create(string url, Guid scoreKey);
        RecommendationEntityCollection Get(uint page, uint pageSize, RecommendationScoreParameters? parameters = null);
        IRecommendation? Get(string url, IRedactionScore score);
        IRecommendation? Get(string url, Guid scoreKey);
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

        public RecommendationEntityCollection Get(uint page, uint pageSize, RecommendationScoreParameters? parameters = null)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var result = _recommendationRepository.Get(page, pageSize, parameters ?? Core.Defaults.Parameters.ScoreParameters);

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

        public IRecommendation? Get(string url, IRedactionScore score)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            return _recommendationRepository.Get(scope.SqlContext.Query<IRecommendation>()
                .Where(e => e.Url == url)
                .Where(e => e.StrategyId == score.Id)).FirstOrDefault();
        }

        public IRecommendation? Get(string url, Guid scoreKey)
        {
            var score = _redactionScoreService.Get(scoreKey);
            if (score is null) throw new ArgumentException("No redaction score exists for given key", nameof(scoreKey));

            return Get(url, score);
        }

        public void Clear()
        {
            using var scope = _scopeProvider.CreateScope();
            _recommendationRepository.Clear();

            scope.Complete();
        }
    }
}
