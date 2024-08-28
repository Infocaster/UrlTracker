using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core
{
    public interface IRecommendationService
    {
        Task CleanupAsync(double upperScore);
        void Clear();
        int Count(DateTime? startDate, DateTime? endDate, Guid[]? recommendationTypes);
        IRecommendation Create(string url, IRedactionScore score);
        IRecommendation Create(string url, Guid scoreKey);
        void Delete(IRecommendation recommendation);
        RecommendationEntityCollection Get(uint page, uint pageSize, RecommendationOrderingOptions orderingOptions, RecommendationFilterOptions filterOptions, RecommendationScoreParameters? parameters = null);
        IRecommendation? Get(string url, IRedactionScore score);
        IRecommendation? Get(string url, Guid scoreKey);
        IRecommendation? Get(int id);
        IEnumerable<IRecommendation> GetMany(int[] ids);
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

        public RecommendationEntityCollection Get(uint page, uint pageSize, RecommendationOrderingOptions orderingOptions, RecommendationFilterOptions filterOptions, RecommendationScoreParameters? parameters = null)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var result = _recommendationRepository.Get(page, pageSize, parameters ?? Core.Defaults.Parameters.ScoreParameters, orderingOptions, filterOptions);

            return result;
        }

        public int Count(DateTime? startDate, DateTime? endDate, Guid[]? recommendationTypes)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            var query = scope.SqlContext.Query<IRecommendation>();

            if (startDate.HasValue) query = query.Where(e => e.UpdateDate >= startDate);
            if (endDate.HasValue) query = query.Where(e => e.UpdateDate <= endDate);
            if (recommendationTypes is not null)
            {
                var redactionScores = _redactionScoreService
                    .GetAll(recommendationTypes)
                    .Select(rs => rs.Id);
                query.WhereIn(e => e.StrategyId, redactionScores);
            }

            var result = _recommendationRepository.Count(query);

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
            var score = _redactionScoreService.Get(scoreKey)
                ?? throw new ArgumentException("No redaction score exists for given key", nameof(scoreKey));

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
            var score = _redactionScoreService.Get(scoreKey)
                ?? throw new ArgumentException("No redaction score exists for given key", nameof(scoreKey));

            return Get(url, score);
        }

        public IRecommendation? Get(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            return _recommendationRepository.Get(id);
        }

        public void Clear()
        {
            using var scope = _scopeProvider.CreateScope();
            _recommendationRepository.Clear();

            scope.Complete();
        }

        public void Delete(IRecommendation recommendation)
        {
            using var scope = _scopeProvider.CreateScope();
            _recommendationRepository.Delete(recommendation);

            scope.Complete();
        }

        public async Task CleanupAsync(double upperScore)
        {
            using var scope = _scopeProvider.CreateScope();
            await _recommendationRepository.CleanupAsync(upperScore, Defaults.Parameters.ScoreParameters);

            scope.Complete();
        }

        public IEnumerable<IRecommendation> GetMany(int[] ids)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return _recommendationRepository.GetMany(ids);
        }
    }
}
