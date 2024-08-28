using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Dtos
{
    [TableName(Defaults.DatabaseSchema.Tables.ClientError)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    [ExcludeFromCodeCoverage]
    internal class ClientErrorDto
    {
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
}
