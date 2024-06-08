using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extensions;

namespace UrlTracker.Core.Database.Migrations
{
    internal partial class M202406071214_MigrateIdsToUniques
        : MigrationBase
    {
        private readonly IIdKeyMap _idKeyMap;

        public M202406071214_MigrateIdsToUniques(
            IMigrationContext context,
            IIdKeyMap idKeyMap)
            : base(context)
        {
            _idKeyMap = idKeyMap;
        }

        protected override void Migrate()
        {
            // all content redirects need to be migrated to use 'uniques' instead of numeric ids for the new U14 backoffice.
            var query = Sql().SelectAll()
                .From<InternalRedirectDto>()
                .Where<InternalRedirectDto>(e => e.TargetStrategy == targetStrategy);

            var dtos = Database.Fetch<InternalRedirectDto>(query);

            if (dtos.Count == 0) return;

            foreach (var dto in dtos)
            {
                var targetSegments = dto.TargetValue.Split(';');

                // Although there shouldn't be any reason, we don't want this process to crash when the first segments happens to not be an integer
                if (!int.TryParse(targetSegments[0], out var contentId))
                {
                    LogUnableToUpdateContentTarget(Logger, dto.Id, $"Unable to parse the target value {targetSegments[0]} to an integer.");
                    continue;
                }

                var key = _idKeyMap.GetKeyForId(contentId, UmbracoObjectTypes.Document);
                if (!key.Success)
                {
                    // This may reasonably happen when the selected content item has been deleted
                    LogUnableToUpdateContentTarget(Logger, dto.Id, "Failed to convert the given numeric id to a unique");
                    continue;
                }

                targetSegments[0] = key.Result.ToString("D").ToUpperInvariant();
                dto.TargetValue = string.Join(";", targetSegments);

                // Couldn't find a bulk or batch update method that allows me to insert a list of dtos.
                Database.Update(dto);
            }
        }

        [LoggerMessage(4000, LogLevel.Warning, "Unable to update redirect {redirectId}: {reason}")]
        private static partial void LogUnableToUpdateContentTarget(ILogger logger, int redirectId, string reason);
    }
}
