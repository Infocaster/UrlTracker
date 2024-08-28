using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Classification
{
    /// <summary>
    /// A base class that provides conversion from GUID redaction scores to entities.
    /// </summary>
    public abstract class UrlClassifierStrategyBase
        : IUrlClassifierStrategy
    {
        /// <inheritdoc />
        public UrlClassifierStrategyBase(IRedactionScoreService redactionScoreService, ILogger<UrlClassifierStrategyBase> logger)
        {
            RedactionScoreService = redactionScoreService;
            Logger = logger;
        }

        /// <summary>
        /// The Logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// The redaction score service
        /// </summary>
        protected IRedactionScoreService RedactionScoreService { get; }

        /// <inheritdoc />
        public IRedactionScore? Classify(Url url)
        {
            var key = DoClassify(url);
            if (key is null) return null;

            var entity = RedactionScoreService.Get(key.Value);
            if (entity is null)
            {
                Logger.LogRedactionScoreNotFound(key.Value);
            }

            return entity;
        }

        /// <summary>
        /// When implemented, this method provides a key for a redaction score, based on the given url
        /// </summary>
        /// <param name="url">The url to classify</param>
        /// <returns>The key of a redaction score if classification was successful, else <see langword="null"/></returns>
        protected abstract Guid? DoClassify(Url url);
    }
}
