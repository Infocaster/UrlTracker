using System;
using System.Collections.Generic;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations
{
    internal class ListRecommendationRequest
        : PaginationRequest
    {
        public string? Query { get; set; }
        public IEnumerable<Guid>? Types { get; set; }

        public RecommendationOrderBy OrderBy { get; set; }
        public bool Desc { get; set; } = true;
    }
}
