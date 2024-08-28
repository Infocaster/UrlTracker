using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Dtos
{
    [TableName(Defaults.DatabaseSchema.Tables.Referrer)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class ReferrerDto
    {
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
}
