using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Dtos
{
    [TableName(Defaults.DatabaseSchema.Tables.RedactionScore)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class RedactionScoreDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("score")]
        public decimal Score { get; set; }

        [Column("recommendationStrategy")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_recommendationStrategy_RecommendationStrategy")]
        public Guid RecommendationStrategy { get; set; }

        [Column("name")]
        [Length(200)]
        public string? Name { get; set; }
    }
}
