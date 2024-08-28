using System;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Web.BackOffice.Controllers;
using UrlTracker.Core;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Website.Models;

namespace UrlTracker.Resources.Website.Controllers
{
    public class UrlTrackerTestController
        : UmbracoAuthorizedApiController
    {
        private readonly IRedactionScoreService _redactionScoreService;
        private readonly IRecommendationService _recommendationService;
        private readonly IUrlClassifierStrategyCollection _urlClassifier;
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerOptions;
        private readonly IScopeProvider _scopeProvider;

        private static Faker<SetRecommendationRequest> requestGenerator = new Faker<SetRecommendationRequest>()
            .RuleFor(e => e.DateTime, f => f.Date.Between(DateTime.Now.AddYears(-1), DateTime.Today))
            .RuleFor(e => e.Visits, f => f.Random.Int(1, 2000))
            .RuleSet("img", set => set.RuleFor(e => e.Url, f => f.Internet.UrlRootedPath(f.PickRandom("jpg", "png", "gif", "ico", "webp", "webm", "mp3"))))
            .RuleSet("file", set => set.RuleFor(e => e.Url, f => f.Internet.UrlRootedPath(f.PickRandom("pdf", "docx", "html"))))
            .RuleSet("page", set => set.RuleFor(e => e.Url, f => f.Internet.UrlRootedPath()))
            .RuleSet("technicalFile", set => set.RuleFor(e => e.Url, f => f.Internet.UrlRootedPath(f.PickRandom("js", "css", "js.map"))));

        public UrlTrackerTestController(IRedactionScoreService redactionScoreService,
                                        IRecommendationService recommendationService,
                                        IUrlClassifierStrategyCollection urlClassifier,
                                        IOptionsMonitor<RequestHandlerSettings> requestHandlerOptions,
                                        IScopeProvider scopeProvider)
        {
            _redactionScoreService = redactionScoreService;
            _recommendationService = recommendationService;
            _urlClassifier = urlClassifier;
            _requestHandlerOptions = requestHandlerOptions;
            _scopeProvider = scopeProvider;
        }

        [HttpGet]
        public IActionResult GetRedactionScores()
        {
            var scores = _redactionScoreService.GetAll();

            var model = scores.Select(s => new RedactionScoreViewModel
            {
                Id = s.Key,
                Name = s.Name,
                Score = s.RedactionScore
            }).ToList();

            return Ok(model);
        }

        [HttpPost]
        public IActionResult SetRedactionScore([FromQuery] Guid id, [FromBody] decimal value)
        {
            var score = _redactionScoreService.Get(id);
            if (score is null) return NotFound();

            score.RedactionScore = value;
            _redactionScoreService.Save(score);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetResults(double c1, double c2, double c3)
        {
            var results = _recommendationService.Get(1, 100, new RecommendationOrderingOptions(), new RecommendationFilterOptions(), new Core.Database.Models.RecommendationScoreParameters
            {
                RedactionFactor = c1,
                VariableFactor = c2,
                TimeFactor = c3
            });

            return Ok(results.Select(r => new RecommendationViewModel
            {
                Strategy = r.Strategy.Key,
                UpdateDate = r.UpdateDate,
                Url = r.Url,
                VariableScore = r.VariableScore
            }));
        }

        [HttpPost]
        public IActionResult ClearRecommendations()
        {
            _recommendationService.Clear();

            return Ok();
        }

        [HttpPost]
        public IActionResult GenerateRandomRecommendations([FromBody] GenerateRandomRequest request)
        {
            var requests = requestGenerator.Generate(25, "default, img").Concat(
                requestGenerator.Generate(25, "default, file")).Concat(
                requestGenerator.Generate(25, "default, page")).Concat(
                requestGenerator.Generate(25, "default, technicalFile")).ToList();

            foreach (var r in requests)
            {
                r.Url = request.BaseUrl.TrimEnd('/') + r.Url;
                SetRecommendation(r);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult SetRecommendation([FromBody] SetRecommendationRequest request)
        {
            var url = Core.Models.Url.Parse(request.Url);

            IRedactionScore strategy = _urlClassifier.Classify(url);

            var optionsValue = _requestHandlerOptions.CurrentValue;
            string urlString = url.ToString(UrlType.Absolute, optionsValue.AddTrailingSlash);
            var recommendation = _recommendationService.GetOrCreate(urlString, strategy);

            recommendation.VariableScore = request.Visits;

            _recommendationService.Save(recommendation);

            using var scope = _scopeProvider.CreateScope();

            scope.Database.Execute($"UPDATE {Defaults.DatabaseSchema.Tables.Recommendation} SET updateDate = @date WHERE url = @url AND recommendationStrategy = @strategy", new
            {
                date = request.DateTime,
                url = urlString,
                strategy = strategy.Id
            });

            scope.Complete();
            return Ok();
        }
    }
}
