using NPoco;
using System;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	[TableName("icUrlTrackerIgnore404")]
	[PrimaryKey("Id", AutoIncrement = true)]
	[ExplicitColumns]
	public class UrlTrackerIgnore404Schema
	{
		[PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
		[Column("Id")]
		public int Id { get; set; }

		[Column("RootNodeId")]
		[NullSetting(NullSetting = NullSettings.Null)]
		public int? RootNodeId { get; set; }

		[Column("Culture")]
		[Length(10)]
		[NullSetting(NullSetting = NullSettings.Null)]
		public string Culture { get; set; }

		[Column("Url")]
		[NullSetting(NullSetting = NullSettings.Null)]
		public string Url { get; set; }

		[Column("Inserted")]
		[Constraint(Default = "getdate()")]
		public DateTime Inserted { get; set; }
	}
}