using System;
using NPoco;

namespace UrlTracker.Core.Database.Models
{
    /* This model is read only!
     *     The purpose of this model is to read data from the UrlTrackerEntry table, aggregated for counting occurrences
     */
    public class UrlTrackerEntryNotFoundAggregate
    {
        [Column("Id"), ComputedColumn]
        public int Id { get; set; }

        [Column("Culture"), ComputedColumn]
        public string Culture { get; set; }

        [Column("OldUrl"), ComputedColumn]
        public string OldUrl { get; set; }

        [Column("RedirectRootNodeId"), ComputedColumn]
        public int? RedirectRootNodeId { get; set; }

        [Column("Is404"), ComputedColumn]
        public bool Is404 { get; set; }

        [Column("Referrer"), ComputedColumn]
        public string Referrer { get; set; }

        [Column("Inserted"), ComputedColumn]
        public DateTime Inserted { get; set; }

        [Column("Occurrences"), ComputedColumn]
        public int Occurrences { get; set; }
    }
}
