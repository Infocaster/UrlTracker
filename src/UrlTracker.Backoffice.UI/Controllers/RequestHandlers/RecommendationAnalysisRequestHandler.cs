using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations.Analysis;
using UrlTracker.Core;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRecommendationAnalysisRequestHandler
    {
        Task<RecommendationHistory?> GetHistoryAsync(RecommendationHistoryRequest request);
        Task<IEnumerable<ReferrerResponse>?> GetMostCommonReferrersAsync(int id);
    }
    internal class RecommendationAnalysisRequestHandler : IRecommendationAnalysisRequestHandler
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IClientErrorService _clientErrorService;

        public RecommendationAnalysisRequestHandler(IRecommendationService recommendationService,
                                                    IClientErrorService clientErrorService)
        {
            _recommendationService = recommendationService;
            _clientErrorService = clientErrorService;
        }

        public async Task<IEnumerable<ReferrerResponse>?> GetMostCommonReferrersAsync(int id)
        {
            var recommendation = _recommendationService.Get(id);
            if (recommendation == null) return null;

            var clientError = await _clientErrorService.GetAsync(recommendation.Url);
            if (clientError == null || clientError.Ignored) return Array.Empty<ReferrerResponse>();

            var referrers = await _clientErrorService.GetClientErrorReferrersAsync(clientError.Id);
            return referrers;
        }

        public async Task<RecommendationHistory?> GetHistoryAsync(RecommendationHistoryRequest request)
        {
            var recommendation = _recommendationService.Get(request.Id);
            if (recommendation == null) return null;

            var clientError = await _clientErrorService.GetAsync(recommendation.Url);
            if (clientError == null || clientError.Ignored) return new RecommendationHistory(recommendation.CreateDate, recommendation.UpdateDate, default, Enumerable.Empty<DailyOccurance>());

            var dailyClientErrors = await _clientErrorService.GetInRangeAsync(clientError.Id, DateTime.Now.AddDays(-request.PastDays), DateTime.Now);
            var inPastDays = FillEmptyDays(dailyClientErrors, request.PastDays);
            var trend = TrendCalculator.GetRecomendationTrend(TrendCalculator.CalculateLinearTrend(dailyClientErrors));

            return new RecommendationHistory(
                clientError.Inserted,
                clientError.LatestOccurrence,
                GetDailyAverage(inPastDays),
                inPastDays,
                trend);
        }

        private static double GetDailyAverage(IEnumerable<DailyOccurance> occurances)
        {
            if (!occurances.Any())
            {
                return 0;
            }

            double totalOccurrences = 0;
            int totalDays = 0;

            foreach (var group in occurances
                .GroupBy(response => response.DateTime.Date))
            {
                totalOccurrences += group.Sum(response => response.Occurances);
                totalDays++;
            }

            return totalOccurrences / totalDays;
        }


        private static IEnumerable<DailyOccurance> FillEmptyDays(IEnumerable<DailyClientErrorResponse> values, int daysInPast)
        {
            DateTime today = DateTime.Today;
            IEnumerable<DateTime> datesInRange = Enumerable.Range(-daysInPast + 1, daysInPast)
                .Select(offset => today.AddDays(offset));

            IEnumerable<DailyOccurance> filledList = datesInRange
                .Select(date =>
                {
                    DailyClientErrorResponse? response = values.FirstOrDefault(r => r.Date == date);
                    return new DailyOccurance(response?.Occurrances ?? 0, date);
                });

            return filledList;
        }
    }
}
