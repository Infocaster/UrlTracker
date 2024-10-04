using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations.Analysis
{
    internal class RecommendationHistoryRequest
    {
        public int Id { get; set; }
        public int PastDays { get; set; }
    }
}
