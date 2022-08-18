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
    [CompatibilityMapperFor(typeof(IClientError))]
    [CompatibilityMapperFor(typeof(ClientErrorEntity))]
    public sealed class ClientErrorMapper
        : BaseMapper
    {
        public ClientErrorMapper(Lazy<ISqlContext> sqlContext, ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> maps)
            : base(sqlContext, maps)
        { }

        protected override void DefineMaps()
        {
            DefineMap<IClientError, ClientErrorDto>(nameof(IClientError.CreateDate), nameof(ClientErrorDto.CreateDate));
            DefineMap<IClientError, ClientErrorDto>(nameof(IClientError.Id), nameof(ClientErrorDto.Id));
            DefineMap<IClientError, ClientErrorDto>(nameof(IClientError.Ignored), nameof(ClientErrorDto.Ignored));
            DefineMap<IClientError, ClientErrorDto>(nameof(IClientError.Key), nameof(ClientErrorDto.Key));
            DefineMap<IClientError, ClientErrorDto>(nameof(IClientError.Url), nameof(ClientErrorDto.Url));
        }
    }
}
