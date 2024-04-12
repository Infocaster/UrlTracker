using System;
using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    [ExcludeFromCodeCoverage]
    public class UrlTargetStrategyMap
        : StrategyMapBase<UrlTargetStrategy>
    {
        /// <inheritdoc/>
        protected override Guid StrategyKey
            => Defaults.DatabaseSchema.RedirectTargetStrategies.Url;

        /// <inheritdoc/>
        public override UrlTargetStrategy Convert(EntityStrategy strategy)
            => new(strategy.Value);

        /// <inheritdoc/>
        protected override EntityStrategy Convert(UrlTargetStrategy strategy)
            => EntityStrategy.UrlTarget(strategy.Url);
    }
}
