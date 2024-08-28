using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202208011724_MigrateOldData
        : MigrationBase
    {
        public M202208011724_MigrateOldData(IMigrationContext context)
            : base(context)
        { }

        protected override void Migrate()
        {
            // transfer referrers
            // Sqlite doesn't understand GUIDS,
            //    so we need to make a roundtrip to the server to actually create new guids for each record
            Logger.LogInformation("Transferring referrers from old table structure");
            Database.BatchFetchAndProcess<OldReferrerDto>(Sql(
            @"SELECT MIN([Inserted]) as Inserted, [Referrer]
            FROM [icUrlTracker]
            WHERE [Referrer] is not null
            GROUP BY [Referrer]
            ORDER BY Inserted"), items =>
            {
                Database.BulkInsertRecords(items.Select(i => new M202206251507_Rework_ReferrerDto
                {
                    CreateDate = i.Inserted,
                    Key = Guid.NewGuid(),
                    Url = i.Referrer
                }));
            });

            // transfer client errors
            Logger.LogInformation("Transferring client error urls from old table structure");
            Database.BatchFetchAndProcess<OldClientErrorDto>(Sql(
            @"SELECT MIN([Inserted]) as Inserted, [OldUrl]
            FROM [icUrlTracker]
            WHERE [Is404] = 1
            GROUP BY [OldUrl]
            ORDER BY Inserted"), items =>
            {
                Database.BulkInsertRecords(items.Select(i => new M202206251507_Rework_ClientErrorDto
                {
                    CreateDate = i.Inserted,
                    Ignored = false,
                    Key = Guid.NewGuid(),
                    Strategy = Defaults.DatabaseSchema.ClientErrorStrategies.NotFound,
                    Url = i.OldUrl
                }));
            });

            // transfer instances of client errors
            Logger.LogInformation("Transferring client error instances from old table structure");
            Database.BatchFetchAndProcess<M202206251507_Rework_ClientError2ReferrerDto>(Sql(
            @"SELECT [ce].[id] as clientError, [re].[id] as referrer, [icUrlTracker].[Inserted] as createDate
            FROM [icUrlTracker]
            left join [urltrackerClientError] ce
            on [ce].[Url] = [icUrlTracker].OldUrl
            left join [urltrackerReferrer] re
            on [re].[Url] = [icUrlTracker].Referrer
            WHERE [icUrlTracker].Is404 = 1 and [icUrlTracker].OldUrl is not null"), items =>
            {
                Database.BulkInsertRecords(items);
            });

            // transfer ignores
            Logger.LogInformation("Transferring ignored client errors from old table structure");
            Database.Execute(
@"update [urltrackerClientError]
set [ignored] = 1
from [icUrlTrackerIgnore404] where [urltrackerClientError].[url] = [icUrlTrackerIgnore404].[Url]");

            Database.BatchFetchAndProcess<OldIgnoreDto>(Sql(
            @"select MIN([ig].[Inserted]) as Inserted,
	        [ig].[Url]
            from [icUrlTrackerIgnore404] as ig
            where [ig].[Url] not in (select [url] from [urltrackerClientError])
            group by [ig].[Url]
            order by Inserted"), items =>
            {
                Database.BulkInsertRecords(items.Select(i => new M202206251507_Rework_ClientErrorDto
                {
                    CreateDate = i.Inserted,
                    Ignored = true,
                    Key = Guid.NewGuid(),
                    Strategy = Defaults.DatabaseSchema.ClientErrorStrategies.NotFound,
                    Url = i.Url
                }));
            });

            // transfer redirects
            Logger.LogInformation("Transferring redirects from old table structure");
            Database.BatchFetchAndProcess<OldRedirectDto>(Sql(
            @"SELECT Inserted, Culture, ForceRedirect, Notes, RedirectPassThroughQueryString, OldRegex, OldUrl, RedirectNodeId, RedirectRootNodeId, RedirectUrl
            FROM [icUrlTracker]
            WHERE [icUrlTracker].[Is404] = 0
            ORDER BY Inserted"), items =>
            {
                Database.BulkInsertRecords(items.Select(i => new M202206251507_Rework_RedirectDto
                {
                    CreateDate = i.Inserted,
                    Culture = i.Culture.DefaultIfNullOrWhiteSpace(null),
                    Force = i.ForceRedirect,
                    Key = Guid.NewGuid(),
                    Notes = i.Notes.DefaultIfNullOrWhiteSpace(null),
                    Permanent = i.RedirectHttpCode == 301,
                    RetainQuery = i.RedirectPassThroughQueryString,
                    SourceRegex = i.OldRegex.DefaultIfNullOrWhiteSpace(null),
                    SourceUrl = i.OldUrl.DefaultIfNullOrWhiteSpace(null),
                    TargetNodeId = i.RedirectNodeId,
                    TargetRootNodeId = i.RedirectRootNodeId,
                    TargetUrl = i.RedirectUrl.DefaultIfNullOrWhiteSpace(null)
                }));
            });
        }

        private class OldReferrerDto
        {
            [Column("Inserted")]
            public DateTime Inserted { get; set; }

            [Column("Referrer")]
            public string Referrer { get; set; } = null!;
        }

        private class OldClientErrorDto
        {
            [Column("Inserted")]
            public DateTime Inserted { get; set; }

            [Column("OldUrl")]
            public string OldUrl { get; set; } = null!;
        }

        private class OldIgnoreDto
        {
            [Column("Inserted")]
            public DateTime Inserted { get; set; }

            [Column("Url")]
            public string Url { get; set; } = null!;
        }

        private class OldRedirectDto
        {
            [Column("Inserted")]
            public DateTime Inserted { get; set; }

            [Column("Culture")]
            public string? Culture { get; set; }

            [Column("ForceRedirect")]
            public bool ForceRedirect { get; set; }

            [Column("Notes")]
            public string? Notes { get; set; }

            [Column("RedirectHttpCode")]
            public int? RedirectHttpCode { get; set; }

            [Column("RedirectPassThroughQueryString")]
            public bool RedirectPassThroughQueryString { get; set; }

            [Column("OldRegex")]
            public string? OldRegex { get; set; }

            [Column("OldUrl")]
            public string? OldUrl { get; set; }

            [Column("RedirectNodeId")]
            public int? RedirectNodeId { get; set; }

            [Column("RedirectRootNodeId")]
            public int? RedirectRootNodeId { get; set; }

            [Column("RedirectUrl")]
            public string? RedirectUrl { get; set; }
        }
    }
}
