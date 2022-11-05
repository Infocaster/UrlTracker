using System;

namespace UrlTracker.Core.Models
{
    /// <summary>
    /// A base implementation of a strategy
    /// </summary>
    public abstract class StrategyBase
        : IStrategyBase
    {
        /// <inheritdoc />
        protected StrategyBase(Guid strategy)
        {
            Strategy = strategy;
        }

        /// <inheritdoc />
        public Guid Strategy { get; }
    }
}
