using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Mappers
{
    [MapperFor(typeof(IReferrer))]
    [ExcludeFromCodeCoverage]
    public sealed class ReferrerMapper
        : BaseMapper
    {
        public ReferrerMapper(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps) : base(sqlContext, maps)
        { }

        protected override void DefineMaps()
        {
            DefineMap<IReferrer, ReferrerDto>(nameof(IReferrer.CreateDate), nameof(ReferrerDto.CreateDate));
            DefineMap<IReferrer, ReferrerDto>(nameof(IReferrer.Id), nameof(ReferrerDto.Id));
            DefineMap<IReferrer, ReferrerDto>(nameof(IReferrer.Key), nameof(ReferrerDto.Key));
            DefineMap<IReferrer, ReferrerDto>(nameof(IReferrer.Url), nameof(ReferrerDto.Url));
        }
    }
}
