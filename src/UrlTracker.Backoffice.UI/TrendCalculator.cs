using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations.Analysis;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.UI
{
    internal static class TrendCalculator
    {

        /// <summary>
        /// this method is performing a mathematical calculation to determine the trend in the data points provided. 
        /// Specifically, it's using a method called linear regression to find a line that best fits the data.
        /// The slope of this line represents the trend.
        /// </summary>
        public static double CalculateLinearTrend(IEnumerable<DailyClientErrorResponse> errorResponses)
        {
            if (errorResponses.Count() < 2) return 0;
            int n = 0;
            double sumX = 0;
            double sumY = 0;
            double sumXY = 0;
            double sumX2 = 0;

            foreach (var response in errorResponses)
            {
                double x = n + 1; // Assuming x values are the index of the data point (1-based)
                double y = response.Occurrances;

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;

                n++;
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);

            return slope;
        }

        public static RecomendationTrend GetRecomendationTrend(double slope)
        {
            if (Math.Abs(slope) < 0.5) // check if absulute value of slope is close to zero
            {
                return RecomendationTrend.Stable;
            }
            else if (slope < 0)
            {
                return RecomendationTrend.Decreasing;
            }
            else
            {
                return RecomendationTrend.Increasing;
            }
        }
    }
}
