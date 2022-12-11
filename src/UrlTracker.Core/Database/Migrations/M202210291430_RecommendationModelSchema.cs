using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Migrations
{
    [TableName(TableName)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class M202210291430_RecommendationModelSchema_RedirectDto
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

        [Column("sourceStrategy")]
        public Guid SourceStrategy { get; set; }

        [Column("sourceValue")]
        [Length(255)]
        public string SourceValue { get; set; } = null!;

        [Column("targetStrategy")]
        public Guid TargetStrategy { get; set; }

        [Column("targetValue")]
        [Length(255)]
        public string TargetValue { get; set; } = null!;

        [Column("retainQuery")]
        public bool RetainQuery { get; set; }

        [Column("permanent")]
        public bool Permanent { get; set; }

        [Column("force")]
        public bool Force { get; set; }
    }

    [TableName(TableName)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class M202210291430_RecommendationModelSchema_RecommendationDto
    {
        public const string TableName = "urltrackerRecommendation";

        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("key")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urltrackerRecommendation_Key")]
        public Guid Key { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("url")]
        public string Url { get; set; } = null!;

        [Column("recommendationStrategy")]
        public int RecommendationStrategy { get; set; }

        [Column("variableScore")]
        public int VariableScore { get; set; }

        [Column("ignore")]
        public bool Ignore { get; set; }
    }

    [TableName(TableName)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class M202210291430_RecommendationModelSchema_RedactionScoreDto
    {
        public const string TableName = "urltrackerRedactionScore";

        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("score")]
        public decimal Score { get; set; }

        [Column("recommendationStrategy")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urltrackerRedactionScore_recommendationStrategy")]
        public Guid RecommendationStrategy { get; set; }

        [Column("name")]
        [Length(200)]
        public string? Name { get; set; }
    }
}
