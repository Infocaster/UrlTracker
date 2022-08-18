using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Mappers;
using UrlTracker.Core.Compatibility;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Mappers
{
    [ExcludeFromCodeCoverage]
    [CompatibilityMapperFor(typeof(IReferrer))]
    [CompatibilityMapperFor(typeof(ReferrerEntity))]
    public sealed class ReferrerMapper
        : BaseMapper
    {
        public ReferrerMapper(Lazy<ISqlContext> sqlContext, ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> maps)
            : base(sqlContext, maps)
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
