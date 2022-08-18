using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Models.Entities;

namespace UrlTracker.Core.Database.Models.Factories
{
    internal static class RedirectFactory
    {
        internal static IRedirect BuildEntity(RedirectDto dto)
        {
            var entity = new RedirectEntity(dto.Culture, dto.TargetRootNodeId, dto.TargetNodeId, dto.TargetUrl, dto.SourceUrl, dto.SourceRegex, dto.RetainQuery, dto.Permanent, dto.Force, dto.Notes);
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
                Culture = entity.Culture,
                TargetRootNodeId = entity.TargetRootNodeId,
                TargetNodeId = entity.TargetNodeId,
                TargetUrl = entity.TargetUrl,
                SourceUrl = entity.SourceUrl,
                SourceRegex = entity.SourceRegex,
                RetainQuery = entity.RetainQuery,
                Permanent = entity.Permanent,
                Force = entity.Force,
                Notes = entity.Notes,
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
