using System;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System.ComponentModel;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
    [TableName("icUrlTracker")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class UrlTrackerSchema
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("OldUrl")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string OldUrl { get; set; }

        [Column("OldUrlQueryString")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string OldUrlQueryString { get; set; }

        [Column("OldRegex")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string OldRexEx { get; set; }

        [Column("RedirectRootNodeId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? RedirectRootNodeId { get; set; }

        [Column("RedirectNodeId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? RedirectNodeId { get; set; }

        [Column("RedirectUrl")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string RedirectUrl { get; set; }

        [Column("RedirectHttpCode")]
        [Constraint(Default = 301)]
        public int RedirectHttpCode { get; set; }

        [Column("RedirectPassThroughQueryString")]
        [Constraint(Default = "1")]
        public bool RedirectPassThroughQueryString { get; set; }

        [Column("ForceRedirect")]
        [Constraint(Default = "0")]
        public bool ForceRedirect { get; set; }

        [Column("Notes")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Notes { get; set; }

        [Column("Is404")]
        [Constraint(Default = false)]
        public bool Is404 { get; set; }

        [Column("Referrer")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Referred { get; set; }

        [Column("Inserted")]
        [Constraint(Default = "getdate()")]
        public DateTime Inserted { get; set; }
    }
}