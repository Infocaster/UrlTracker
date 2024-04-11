using System;
using System.Diagnostics.CodeAnalysis;
using NPoco;

namespace UrlTracker.Core.Database.Dtos
{
    [ExcludeFromCodeCoverage]
    internal class ClientErrorMetaDataDto
    {
        [Column(Defaults.DatabaseSchema.AggregateColumns.TotalOccurrences)]
        public int? TotalOccurrances { get; set; }

        [Column(Defaults.DatabaseSchema.AggregateColumns.MostCommonReferrer)]
        public string? MostCommonReferrer { get; set; }

        [Column(Defaults.DatabaseSchema.AggregateColumns.MostRecentOccurrence)]
        public DateTime? MostRecentOccurrance { get; set; }

        [Column("clientError")]
        public int ClientError { get; set; }
    }
}
