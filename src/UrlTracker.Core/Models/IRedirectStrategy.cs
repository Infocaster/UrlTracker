using System;

namespace UrlTracker.Core.Models
{
    /// <summary>
    /// This base interface provides identification for source and target strategies
    /// </summary>
    public interface IStrategyBase
    {
        /// <summary>
        /// The unique identifier of the strategy
        /// </summary>
        Guid Strategy { get; }
    }

    /// <summary>
    /// A strategy for matching incoming URLs
    /// </summary>
    public interface ISourceStrategy
        : IStrategyBase
    { }

    /// <summary>
    /// A strategy for creating outgoing URLs
    /// </summary>
    public interface ITargetStrategy
        : IStrategyBase
    { }
}
