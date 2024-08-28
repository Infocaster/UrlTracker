using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;

namespace UrlTracker.Core.Database.Dtos
{
    [ExcludeFromCodeCoverage]
    [ExplicitColumns]
    internal class DailyClientErrorDto
    {
        [Column("occurances")]
        public int Occurrances { get; set; }

        [Column("dateOnly")]
        public DateTime Date { get; set; }

    }
}
