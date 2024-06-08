using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Migrations
{
    internal partial class M202406071214_MigrateIdsToUniques
    {
        // NOTE: this is copied from the constants, because migrations need to be standalone.
        // They cannot rely on objects that are used in business logic,
        // as those are prone to change and migrations may NEVER change
        private readonly Guid targetStrategy = new("e9e3a702-54f7-42ae-aadd-5b04185da988");

        [TableName(TableName)]
        [PrimaryKey("id")]
        [ExplicitColumns]
        [ExcludeFromCodeCoverage]
        private sealed class InternalRedirectDto
        {
            private const string TableName = "UrlTrackerRedirect";

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
}
