using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Validation.Attributes;

namespace UrlTracker.Core.Models
{
    /// <summary>
    /// A base implementation for redirect models to content
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ContentTargetStrategyBase
        : ITargetStrategy
    {
        /// <inheritdoc />
        protected ContentTargetStrategyBase(IPublishedContent? content)
        {
            Content = content;
        }

        /// <summary>
        /// The content to redirect to
        /// </summary>
        [Required]
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
            : base(content)
        {
            Culture = culture;
        }

        /// <summary>
        /// The culture of this content item to redirect to
        /// </summary>
        [ValidCultureFormat]
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
            return HashCode.Combine(Content, Culture);
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
            : base(content)
        { }
    }

    /// <summary>
    /// A redirect strategy that redirects to a specific URL
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UrlTargetStrategy
        : ITargetStrategy
    {
        /// <inheritdoc />
        public UrlTargetStrategy(string url)
        {
            Url = url;
        }

        /// <summary>
        /// The URL to redirect to
        /// </summary>
        [Required]
        public string Url { get; }
    }
}
