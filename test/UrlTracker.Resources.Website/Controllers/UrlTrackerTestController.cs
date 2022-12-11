using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using UrlTracker.Core;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Resources.Website.Models;

namespace UrlTracker.Resources.Website.Controllers
{
    public class UrlTrackerTestController
        : UmbracoAuthorizedApiController
    {
        private readonly IRedactionScoreService _redactionScoreService;

        public UrlTrackerTestController(IRedactionScoreService redactionScoreService)
        {
            _redactionScoreService = redactionScoreService;
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
        public IActionResult SetRedactionScore([FromQuery]Guid id, [FromBody]decimal value)
        {
            var score = _redactionScoreService.Get(id);
            if (score is null) return NotFound();

            score.RedactionScore = value;

            return Ok();
        }
    }
}
