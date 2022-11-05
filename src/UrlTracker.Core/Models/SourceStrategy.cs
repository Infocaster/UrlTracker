using System;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Models
{
    /// <summary>
    /// A base class for all the native URL Tracker source strategies
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class SourceStrategyBase
        : StrategyBase, ISourceStrategy
    {
        /// <inheritdoc />
        protected SourceStrategyBase(Guid strategy, string value)
            : base(strategy)
        {
            Value = value;
        }

        /// <summary>
        /// The parameter that defines the behaviour of this strategy
        /// </summary>
        public virtual string Value { get; set; }
    }

    /// <summary>
    /// A strategy model for static URL matching
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UrlSourceStrategy
        : SourceStrategyBase
    {
        /// <inheritdoc />
        public UrlSourceStrategy(string value)
            : base(Defaults.DatabaseSchema.RedirectSourceStrategies.Url, value)
        { }
    }

    /// <summary>
    /// A strategy model for regular expression URL matching
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RegexSourceStrategy
        : SourceStrategyBase
    {
        /// <inheritdoc />
        public RegexSourceStrategy(string value)
            : base(Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression, value)
        { }
    }
}
