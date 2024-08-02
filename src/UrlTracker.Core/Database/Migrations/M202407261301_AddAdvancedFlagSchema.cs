using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extensions;

namespace UrlTracker.Core.Database.Migrations
{
    internal partial class M202407261301_AddAdvancedFlag
    {
        [TableName(TableName)]
        [PrimaryKey("id")]
        [ExplicitColumns]
        [ExcludeFromCodeCoverage]
        internal class RedirectDto
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

            [Column("advanced")]
            [Constraint(Default = false)]
            public bool Advanced { get; set; }
        }
    }
}
