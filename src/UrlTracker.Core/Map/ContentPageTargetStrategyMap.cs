using System;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    internal class ContentPageTargetStrategyMap
        : StrategyMapBase<ContentPageTargetStrategy>
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public ContentPageTargetStrategyMap(IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        protected override Guid StrategyKey
            => Defaults.DatabaseSchema.RedirectTargetStrategies.Content;

        public override ContentPageTargetStrategy Convert(EntityStrategy strategy)
        {
            var components = strategy.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
            string? culture = components.Length > 1 ? components[1] : null;

            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            IPublishedContent? content = null;
            if (int.TryParse(components[0], out var contentId))
            {
                content = cref.GetContentById(contentId);
            }
            else if (Guid.TryParse(components[0], out var contentGuid))
            {
                content = cref.GetContentById(contentGuid);
            }

            return new ContentPageTargetStrategy(content, culture);
        }

        protected override EntityStrategy Convert(ContentPageTargetStrategy strategy)
        {
            var value = strategy.Content?.Key.ToString() ?? throw new ArgumentException("When converting to a simplified model, the content item must exist", nameof(strategy));

            if (strategy.Culture != null)
                value += ";" + strategy.Culture;

            return EntityStrategy.ContentTarget(value);
        }
    }
}
