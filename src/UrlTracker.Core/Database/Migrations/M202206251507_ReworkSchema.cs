using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Migrations
{
    [TableName(TableName)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class M202206251507_Rework_RedirectDto
    {
        public const string TableName = "urltrackerRedirect";

        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("key")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerRedirect_Key")]
        public Guid Key { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("culture")]
        [Length(10)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? Culture { get; set; }

        [Column("targetRootNodeId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? TargetRootNodeId { get; set; }

        [Column("targetNodeId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? TargetNodeId { get; set; }

        [Column("targetUrl")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? TargetUrl { get; set; }

        [Column("sourceUrl")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? SourceUrl { get; set; }

        [Column("sourceRegex")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? SourceRegex { get; set; }

        [Column("retainQuery")]
        public bool RetainQuery { get; set; }

        [Column("permanent")]
        public bool Permanent { get; set; }

        [Column("force")]
        public bool Force { get; set; }

        [Column("notes")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? Notes { get; set; }
    }

    [TableName(TableName)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    public class M202206251507_Rework_ClientErrorDto
    {
        public const string TableName = "urltrackerClientError";

        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("key")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerClientError_Key")]
        public Guid Key { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("url")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerClientError_Url")]
        public string Url { get; set; } = null!;

        [Column("ignored")]
        public bool Ignored { get; set; }

        [Column("strategy")]
        public Guid Strategy { get; set; }
    }

    [TableName(TableName)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    public class M202206251507_Rework_ReferrerDto
    {
        public const string TableName = "urltrackerReferrer";

        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("key")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerReferrer_Key")]
        public Guid Key { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("url")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerReferrer_Url")]
        public string Url { get; set; } = null!;
    }

    [TableName(TableName)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    public class M202206251507_Rework_ClientError2ReferrerDto
    {
        public const string TableName = "urltrackerClientError2Referrer";

        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("clientError")]
        [ForeignKey(typeof(M202206251507_Rework_ClientErrorDto), OnDelete = Rule.Cascade)]
        [Index(IndexTypes.NonClustered, ForColumns = "clientError,referrer,createDate", Name = "IX_urltrackerClientError2Referrer_ClientErrorReferrer")]
        public int ClientError { get; set; }

        [Column("referrer")]
        [ForeignKey(typeof(M202206251507_Rework_ReferrerDto))]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? Referrer { get; set; }
    }
}
