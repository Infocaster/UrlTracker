using System;
using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    [ExcludeFromCodeCoverage]
    public class UrlSourceStrategyMap
        : StrategyMapBase<UrlSourceStrategy>
    {
        /// <inheritdoc/>
        protected override Guid StrategyKey
            => Defaults.DatabaseSchema.RedirectSourceStrategies.Url;

        /// <inheritdoc/>
        public override UrlSourceStrategy Convert(EntityStrategy strategy)
            => new(strategy.Value);

        /// <inheritdoc/>
        protected override EntityStrategy Convert(UrlSourceStrategy strategy)
            => EntityStrategy.UrlSource(strategy.Value);
    }
}
