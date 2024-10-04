using System;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Factories
{
    internal class RecommendationFactory
    {
        internal static IRecommendation BuildEntity(RecommendationDto dto, IRedactionScore strategy)
        {
            if (strategy.Id != dto.RecommendationStrategy) throw new ArgumentException("strategy keys do not match");

            var entity = new RecommendationEntity(dto.Url, strategy);
            try
            {
                entity.DisableChangeTracking();

                entity.CreateDate = dto.CreateDate;
                entity.UpdateDate = dto.UpdateDate;
                entity.Id = dto.Id;
                entity.Ignore = dto.Ignore;
                entity.Key = dto.Key;
                entity.VariableScore = dto.VariableScore;

                entity.ResetDirtyProperties();
                return entity;
            }
            finally
            {
                entity.EnableChangeTracking();
            }
        }

        internal static IRedactionScore BuildEntity(RedactionScoreDto dto)
        {
            var entity = new RedactionScoreEntity();
            try
            {
                entity.DisableChangeTracking();
                entity.Id = dto.Id;
                entity.Key = dto.RecommendationStrategy;
                entity.RedactionScore = dto.Score;
                entity.Name = dto.Name;

                entity.ResetDirtyProperties();
                return entity;
            }
            finally
            {
                entity.EnableChangeTracking();
            }
        }

        internal static RecommendationDto BuildDto(IRecommendation entity)
        {
            var dto = new RecommendationDto
            {
                CreateDate = entity.CreateDate,
                Ignore = entity.Ignore,
                RecommendationStrategy = entity.StrategyId,
                VariableScore = entity.VariableScore,
                UpdateDate = entity.UpdateDate,
                Url = entity.Url,
                Key = entity.Key
            };

            if (entity.HasIdentity)
            {
                dto.Id = entity.Id;
            }

            return dto;
        }

        internal static RedactionScoreDto BuildDto(IRedactionScore entity)
        {
            var dto = new RedactionScoreDto
            {
                RecommendationStrategy = entity.Key,
                Score = entity.RedactionScore,
                Name = entity.Name,
            };

            if (entity.HasIdentity)
            {
                dto.Id = entity.Id;
            }

            return dto;
        }
    }
}
