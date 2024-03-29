﻿using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Models.Entities;

namespace UrlTracker.Core.Database.Mappers
{
    [MapperFor(typeof(IRedirect))]
    [ExcludeFromCodeCoverage]
    public sealed class RedirectMapper
        : BaseMapper
    {
        public RedirectMapper(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps)
            : base(sqlContext, maps)
        { }

        protected override void DefineMaps()
        {
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.CreateDate), nameof(RedirectDto.CreateDate));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Culture), nameof(RedirectDto.Culture));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Force), nameof(RedirectDto.Force));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Id), nameof(RedirectDto.Id));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Key), nameof(RedirectDto.Key));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Notes), nameof(RedirectDto.Notes));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.Permanent), nameof(RedirectDto.Permanent));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.RetainQuery), nameof(RedirectDto.RetainQuery));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.SourceRegex), nameof(RedirectDto.SourceRegex));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.SourceUrl), nameof(RedirectDto.SourceUrl));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.TargetNodeId), nameof(RedirectDto.TargetNodeId));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.TargetRootNodeId), nameof(RedirectDto.TargetRootNodeId));
            DefineMap<IRedirect, RedirectDto>(nameof(IRedirect.TargetUrl), nameof(RedirectDto.TargetUrl));
        }
    }
}
