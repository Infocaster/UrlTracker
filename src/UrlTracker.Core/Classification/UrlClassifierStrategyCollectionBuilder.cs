using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Classification
{
    /// <inheritdoc />
    public class UrlClassifierStrategyCollectionBuilder
        : OrderedCollectionBuilderBase<UrlClassifierStrategyCollectionBuilder, UrlClassifierStrategyCollection, IUrlClassifierStrategy>
    {
        /// <inheritdoc />
        protected override UrlClassifierStrategyCollectionBuilder This => this;
    }

    /// <summary>
    /// This type wraps all strategies for classifying urls into a single service
    /// </summary>
    public interface IUrlClassifierStrategyCollection
    {
        /// <summary>
        /// When implemented, returns a redaction score for a given url.
        /// </summary>
        /// <param name="url">The url to classify</param>
        /// <returns>The redaction score that corresponds to the incoming url's properties</returns>
        IRedactionScore Classify(Url url);
    }

    /// <inheritdoc />
    public class UrlClassifierStrategyCollection
        : BuilderCollectionBase<IUrlClassifierStrategy>, IUrlClassifierStrategyCollection
    {
        private readonly IFallbackUrlClassifier _fallback;

        /// <inheritdoc />
        public UrlClassifierStrategyCollection(Func<IEnumerable<IUrlClassifierStrategy>> items, IFallbackUrlClassifier fallback)
            : base(items)
        {
            _fallback = fallback;
        }

        /// <inheritdoc />
        public IRedactionScore Classify(Url url)
        {
            foreach (var classifier in this)
            {
                var score = classifier.Classify(url);
                if (score is not null) return score;
            }

            return _fallback.Classify(url);
        }
    }
}
