using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Factories
{
    [ExcludeFromCodeCoverage]
    internal static class ClientErrorFactory
    {
        internal static IClientError BuildEntity(ExtendedClientErrorDto dto)
        {
            var entity = new ClientErrorEntity(dto.Url, dto.Ignored, dto.Strategy, dto.TotalOccurrances ?? default, dto.MostCommonReferrer, dto.MostRecentOccurrance ?? default);
            try
            {
                entity.DisableChangeTracking();

                entity.CreateDate = dto.CreateDate;
                entity.Key = dto.Key;
                entity.Id = dto.Id;
                entity.Strategy = dto.Strategy;

                entity.ResetDirtyProperties(false);
                return entity;
            }
            finally
            {
                entity.EnableChangeTracking();
            }
        }

        internal static IReferrer BuildEntity(ReferrerDto dto)
        {
            var entity = new ReferrerEntity(dto.Url);
            try
            {
                entity.DisableChangeTracking();

                entity.CreateDate = dto.CreateDate;
                entity.Key = dto.Key;
                entity.Id = dto.Id;

                entity.ResetDirtyProperties(false);
                return entity;
            }
            finally
            {
                entity.EnableChangeTracking();
            }
        }

        internal static ClientErrorDto BuildDto(IClientError entity)
        {
            var dto = new ClientErrorDto
            {
                CreateDate = entity.CreateDate,
                Ignored = entity.Ignored,
                Url = entity.Url,
                Key = entity.Key,
                Strategy = entity.Strategy
            };

            if (entity.HasIdentity)
            {
                dto.Id = entity.Id;
            }

            return dto;
        }

        internal static ReferrerDto BuildDto(IReferrer entity)
        {
            var dto = new ReferrerDto
            {
                CreateDate = entity.CreateDate,
                Key = entity.Key,
                Url = entity.Url
            };

            if (entity.HasIdentity)
            {
                dto.Id = entity.Id;
            }

            return dto;
        }
    }
}
