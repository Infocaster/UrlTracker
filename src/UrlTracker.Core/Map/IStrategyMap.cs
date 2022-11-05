using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    /// <summary>
    /// When implemented, this type can convert specific strategies between simplified and complex versions
    /// </summary>
    public interface IStrategyMap
    {
        /// <summary>
        /// Check if the strategy is suitable for conversion by this converter
        /// </summary>
        /// <param name="strategyBase">The strategy to check</param>
        /// <returns><see langword="true"/> if this converter can convert the given strategy, otherwise <see langword="false"/></returns>
        bool CanHandle(IStrategyBase strategyBase);

        /// <inheritdoc cref="CanHandle(IStrategyBase)"/>
        bool CanHandle(EntityStrategy strategyBase);

        /// <summary>
        /// Do the conversion
        /// </summary>
        /// <param name="strategy">The strategy to convert</param>
        /// <returns>The result of the conversion</returns>
        IStrategyBase Convert(EntityStrategy strategy);

        /// <inheritdoc cref="Convert(EntityStrategy)"/>
        EntityStrategy Convert(IStrategyBase strategy);
    }

    /// <summary>
    /// When implemented, this type can convert specific strategies between simplified and complex versions
    /// </summary>
    /// <typeparam name="T">The type of strategy to convert to and from</typeparam>
    public interface IStrategyMap<out T>
        : IStrategyMap
        where T : IStrategyBase
    {
        /// <inheritdoc cref="IStrategyMap.Convert(EntityStrategy)"/>
        new T Convert(EntityStrategy strategy);
    }
}