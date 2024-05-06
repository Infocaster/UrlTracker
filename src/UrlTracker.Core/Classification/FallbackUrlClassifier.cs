using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Exceptions;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Classification
{
    /// <inheritdoc />
    public class FallbackUrlClassifier : IFallbackUrlClassifier
    {
        private readonly IRedactionScoreService _redactionScoreService;
        private readonly ILogger<FallbackUrlClassifier> _logger;

        /// <inheritdoc />
        public FallbackUrlClassifier(IRedactionScoreService redactionScoreService, ILogger<FallbackUrlClassifier> logger)
        {
            _redactionScoreService = redactionScoreService;
            _logger = logger;
        }

        /// <inheritdoc />
        public IRedactionScore Classify(Url url)
        {
            _logger.LogClassificationFailed();

            var entity = _redactionScoreService.Get(Defaults.DatabaseSchema.RedactionScores.Page);
            if (entity is null) throw new PanicException("The fallback redaction score could not be found");

            return entity;
        }
    }
}
