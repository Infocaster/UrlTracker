using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Mappers
{
    [MapperFor(typeof(IRecommendation))]
    [ExcludeFromCodeCoverage]
    internal class RecommendationMapper
        : BaseMapper
    {
        public RecommendationMapper(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps)
            : base(sqlContext, maps)
        { }

        protected override void DefineMaps()
        {
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.CreateDate), nameof(RecommendationDto.CreateDate));
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.Id), nameof(RecommendationDto.Id));
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.Ignore), nameof(RecommendationDto.Ignore));
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.Key), nameof(RecommendationDto.Key));
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.StrategyId), nameof(RecommendationDto.RecommendationStrategy));
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.UpdateDate), nameof(RecommendationDto.UpdateDate));
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.Url), nameof(RecommendationDto.Url));
            DefineMap<IRecommendation, RecommendationDto>(nameof(IRecommendation.VariableScore), nameof(RecommendationDto.VariableScore));
        }
    }
}
