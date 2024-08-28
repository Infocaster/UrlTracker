using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UrlTracker.Core.Database.Dtos
{
    [ExcludeFromCodeCoverage]
    [ExplicitColumns]
    internal class OccurrancesDto
    {
        [Column("occurrances")]
        public int Occurrances { get; set; }

        [Column("url")]
        public string Url { get; set; } = null!;
    }
}
