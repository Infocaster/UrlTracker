using System;
using System.Data;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Dtos
{
    [TableName(Defaults.DatabaseSchema.Tables.ClientError2Referrer)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class ClientError2ReferrerDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("clientError")]
        [ForeignKey(typeof(ClientErrorDto), OnDelete = Rule.Cascade)]
        [Index(IndexTypes.NonClustered, ForColumns = "clientError,referrer,createDate", Name = "IX_urltrackerClientError2Referrer_ClientErrorReferrer")]
        public int ClientError { get; set; }

        [Column("referrer")]
        [ForeignKey(typeof(ReferrerDto))]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? Referrer { get; set; }
    }
}
