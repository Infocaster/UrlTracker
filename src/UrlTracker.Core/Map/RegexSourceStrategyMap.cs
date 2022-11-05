using System;
using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    [ExcludeFromCodeCoverage]
    public class RegexSourceStrategyMap
        : StrategyMapBase<RegexSourceStrategy>
    {
        /// <inheritdoc/>
        protected override Guid StrategyKey
            => Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression;

        /// <inheritdoc/>
        public override RegexSourceStrategy Convert(EntityStrategy strategy)
            => new(strategy.Value);

        /// <inheritdoc/>
        protected override EntityStrategy Convert(RegexSourceStrategy strategy)
            => EntityStrategy.RegexSource(strategy.Value);
    }
}
