using System;
using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Map
{
    /// <summary>
    /// A base implementation for the strategy mapper that implements the common features
    /// </summary>
    /// <typeparam name="T">The strategy type that this mapper can produce</typeparam>
    public abstract class StrategyMapBase<T>
        : IStrategyMap<T>
        where T : notnull
    {
        /// <summary>
        /// The unique identifier of the strategy that this type can convert
        /// </summary>
        protected abstract Guid StrategyKey { get; }

        /// <inheritdoc />
        public bool CanHandle(object strategyBase)
            => strategyBase is T;

        /// <inheritdoc />
        public bool CanHandle(EntityStrategy strategyBase)
            => strategyBase.Strategy == StrategyKey;

        /// <inheritdoc />
        public abstract T Convert(EntityStrategy strategy);

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public EntityStrategy Convert(object strategy)
            => Convert((T)strategy);

        /// <inheritdoc cref="Convert(object)"/>
        protected abstract EntityStrategy Convert(T strategy);

        [ExcludeFromCodeCoverage]
        object IStrategyMap.Convert(EntityStrategy strategy)
            => Convert(strategy);
    }
}
