﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations
{
    [DataContract]
    internal class RecommendationCollectionResponse
        : PagedCollectionResponseBase<RecommendationResponse>
    {
        public RecommendationCollectionResponse(IEnumerable<RecommendationResponse> results, long total)
            : base(results, total)
        { }
    }
}
