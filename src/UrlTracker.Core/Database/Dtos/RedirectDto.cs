using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Dtos
{
    [TableName(Defaults.DatabaseSchema.Tables.Redirect)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    public class RedirectDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("key")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerRedirect_Key")]
        public Guid Key { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("culture")]
        [Length(11)]
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
}
