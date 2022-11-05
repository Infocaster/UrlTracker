using System;
using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    /// <summary>
    /// A base implementation for the strategy mapper that implements the common features
    /// </summary>
    /// <typeparam name="T">The strategy type that this mapper can produce</typeparam>
    public abstract class StrategyMapBase<T>
        : IStrategyMap<T>
        where T : IStrategyBase
    {
        /// <summary>
        /// The unique identifier for the specific strategy that this mapper converts
        /// </summary>
        protected abstract Guid StrategyKey { get; }

        /// <inheritdoc />
        public bool CanHandle(IStrategyBase strategyBase)
            => strategyBase.Strategy == StrategyKey && strategyBase is T;

        /// <inheritdoc />
        public bool CanHandle(EntityStrategy strategyBase)
            => strategyBase.Strategy == StrategyKey;

        /// <inheritdoc />
        public abstract T Convert(EntityStrategy strategy);

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public EntityStrategy Convert(IStrategyBase strategy)
            => Convert((T)strategy);

        /// <inheritdoc cref="Convert(IStrategyBase)"/>
        protected abstract EntityStrategy Convert(T strategy);

        [ExcludeFromCodeCoverage]
        IStrategyBase IStrategyMap.Convert(EntityStrategy strategy)
            => Convert(strategy);
    }
}
