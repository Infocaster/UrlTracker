using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Mappers
{
    [MapperFor(typeof(IRedirect))]
    [ExcludeFromCodeCoverage]
    internal sealed class RedirectMapper
        : BaseMapper
    {
        public RedirectMapper(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps)
            : base(sqlContext, maps)
        { }

        protected override void DefineMaps()
        {
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.CreateDate), nameof(RedirectDto.CreateDate));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Force), nameof(RedirectDto.Force));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Id), nameof(RedirectDto.Id));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Key), nameof(RedirectDto.Key));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Permanent), nameof(RedirectDto.Permanent));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.RetainQuery), nameof(RedirectDto.RetainQuery));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.SourceStrategy), nameof(RedirectDto.SourceStrategy));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.SourceValue), nameof(RedirectDto.SourceValue));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.TargetStrategy), nameof(RedirectDto.TargetStrategy));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.TargetValue), nameof(RedirectDto.TargetValue));
        }
    }
}
