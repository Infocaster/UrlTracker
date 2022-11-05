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
    internal class RedirectDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("key")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_urlTrackerRedirect_Key")]
        public Guid Key { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("retainQuery")]
        public bool RetainQuery { get; set; }

        [Column("permanent")]
        public bool Permanent { get; set; }

        [Column("force")]
        public bool Force { get; set; }

        [Column("sourceStrategy")]
        public Guid SourceStrategy { get; set; }

        [Column("sourceValue")]
        public string SourceValue { get; set; } = null!;

        [Column("targetStrategy")]
        public Guid TargetStrategy { get; set; }

        [Column("targetValue")]
        public string TargetValue { get; set; } = null!;
    }
}
