using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database.Factories
{
    [ExcludeFromCodeCoverage]
    internal static class RedirectFactory
    {
        internal static IRedirect BuildEntity(RedirectDto dto)
        {
            var entity = new RedirectEntity(dto.RetainQuery, dto.Permanent, dto.Force, new EntityStrategy(dto.SourceStrategy, dto.SourceValue), new EntityStrategy(dto.TargetStrategy, dto.TargetValue));
            try
            {
                entity.DisableChangeTracking();

                entity.CreateDate = dto.CreateDate;
                entity.Id = dto.Id;
                entity.Key = dto.Key;

                entity.ResetDirtyProperties(false);
                return entity;
            }
            finally
            {
                entity.EnableChangeTracking();
            }
        }

        internal static RedirectDto BuildDto(IRedirect entity)
        {
            var dto = new RedirectDto
            {
                RetainQuery = entity.RetainQuery,
                Permanent = entity.Permanent,
                Force = entity.Force,
                TargetValue = entity.Target.Value,
                SourceValue = entity.Source.Value,
                SourceStrategy = entity.Source.Strategy,
                TargetStrategy = entity.Target.Strategy,
                CreateDate = entity.CreateDate,
                Key = entity.Key,
            };

            if (entity.HasIdentity)
            {
                dto.Id = entity.Id;
            }

            return dto;
        }
    }
}
