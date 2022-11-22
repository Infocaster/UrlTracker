using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Dtos
{
    [TableName(Defaults.DatabaseSchema.Tables.Recommendation)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class RecommendationDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("key")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerRedirect_Key")]
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
}
