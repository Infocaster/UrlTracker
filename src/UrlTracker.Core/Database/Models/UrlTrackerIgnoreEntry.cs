using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;

namespace UrlTracker.Core.Database.Models
{
    [DebuggerDisplay("{Id} | {RootNodeId} | {Culture} | {Url}")]
    [TableName("icUrlTrackerIgnore404")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExcludeFromCodeCoverage]
    public class UrlTrackerIgnoreEntry
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        public int? Id { get; set; }

        [Column("RootNodeId"), NullSetting(NullSetting = NullSettings.Null)]
        public int? RootNodeId { get; set; }

        [Column("Culture"), Length(10), NullSetting(NullSetting = NullSettings.Null)]
        public string Culture { get; set; }

        [Column("Url"), Length(255), NullSetting(NullSetting = NullSettings.Null)]
        public string Url { get; set; }

        [Column("Inserted"), Constraint(Default = SystemMethods.CurrentDateTime), ComputedColumn]
        public DateTime Inserted { get; set; }
    }
}
