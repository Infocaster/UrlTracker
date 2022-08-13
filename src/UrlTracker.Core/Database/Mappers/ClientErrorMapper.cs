using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Mappers
{
    [MapperFor(typeof(IClientError))]
    [ExcludeFromCodeCoverage]
    public sealed class ClientErrorMapper
        : BaseMapper
    {
        public ClientErrorMapper(Lazy<ISqlContext> sqlContext, MapperConfigurationStore maps)
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
