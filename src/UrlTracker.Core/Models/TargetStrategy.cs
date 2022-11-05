using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Models
{
    /// <summary>
    /// A base implementation for redirect models to content
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ContentTargetStrategyBase
        : StrategyBase, ITargetStrategy
    {
        /// <inheritdoc />
        protected ContentTargetStrategyBase(Guid strategy, IPublishedContent? content) : base(strategy)
        {
            Content = content;
        }

        /// <summary>
        /// The content to redirect to
        /// </summary>
        public virtual IPublishedContent? Content { get; }
    }

    /// <summary>
    /// A redirect strategy that redirects to content pages, optionally of a particular culture
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContentPageTargetStrategy
        : ContentTargetStrategyBase, IEquatable<ContentPageTargetStrategy>
    {
        /// <inheritdoc />
        public ContentPageTargetStrategy(IPublishedContent? content, string? culture)
            : base(Defaults.DatabaseSchema.RedirectTargetStrategies.Content, content)
        {
            Culture = culture;
        }

        /// <summary>
        /// The culture of this content item to redirect to
        /// </summary>
        public string? Culture { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is ContentPageTargetStrategy other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(ContentPageTargetStrategy? other)
        {
            return other is not null && Content == other.Content && Culture == other.Culture;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Content, Culture, Strategy);
        }
    }

    /// <summary>
    /// A redirect strategy that redirects to media items
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaTargetStrategy
        : ContentTargetStrategyBase
    {
        /// <inheritdoc />
        public MediaTargetStrategy(IPublishedContent content)
            : base(Defaults.DatabaseSchema.RedirectTargetStrategies.Media, content)
        { }
    }

    /// <summary>
    /// A redirect strategy that redirects to a specific URL
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UrlTargetStrategy
        : StrategyBase, ITargetStrategy
    {
        /// <inheritdoc />
        public UrlTargetStrategy(Url url)
            : base(Defaults.DatabaseSchema.RedirectTargetStrategies.Url)
        {
            Url = url;
        }

        /// <inheritdoc />
        public UrlTargetStrategy(string url)
            : this(Url.Parse(url))
        { }

        /// <summary>
        /// The URL to redirect to
        /// </summary>
        public Url Url { get; }
    }
}
