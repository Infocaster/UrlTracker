using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Models
{
    /// <summary>
    /// A base class for all the native URL Tracker source strategies
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class SourceStrategyBase
        : ISourceStrategy
    {
        /// <inheritdoc />
        protected SourceStrategyBase(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The parameter that defines the behaviour of this strategy
        /// </summary>
        [Required]
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
            : base(value)
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
            : base(value)
        { }
    }
}
