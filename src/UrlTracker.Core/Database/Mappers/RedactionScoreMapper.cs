using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Mappers
{
    [MapperFor(typeof(IRedactionScore))]
    [ExcludeFromCodeCoverage]
    internal sealed class RedactionScoreMapper
        : BaseMapper
    {
        public RedactionScoreMapper(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps)
            : base(sqlContext, maps)
        { }

        protected override void DefineMaps()
        {
            DefineMap<IRedactionScore, RedactionScoreDto>(nameof(IRedactionScore.Id), nameof(RedactionScoreDto.Id));
            DefineMap<IRedactionScore, RedactionScoreDto>(nameof(IRedactionScore.Key), nameof(RedactionScoreDto.RecommendationStrategy));
            DefineMap<IRedactionScore, RedactionScoreDto>(nameof(IRedactionScore.RedactionScore), nameof(RedactionScoreDto.Score));
            DefineMap<IRedactionScore, RedactionScoreDto>(nameof(IRedactionScore.Name), nameof(RedactionScoreDto.Name));
        }
    }
}
