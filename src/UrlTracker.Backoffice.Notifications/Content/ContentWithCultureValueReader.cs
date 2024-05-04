using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using UrlTracker.Core;

namespace UrlTracker.Backoffice.Notifications.Content
{
    /// <summary>
    /// An implementation of <see cref="IContentValueReader"/> that reads content values based on a given culture
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContentWithCultureValueReader : IContentValueReader
    {
        private readonly IPublishedValueFallback _publishedValueFallback;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IContent _content;
        private readonly string? _culture;

        /// <inheritdoc />
        public ContentWithCultureValueReader(IPublishedValueFallback publishedValueFallback, IVariationContextAccessor variationContextAccessor, IContent content, ContentCultureInfos? culture = null)
        {
            _publishedValueFallback = publishedValueFallback;
            _variationContextAccessor = variationContextAccessor;
            _content = content;
            _culture = culture?.Culture.NormalizeCulture();
        }

        /// <inheritdoc/>
        public string? GetName()
        {
            return _content.GetCultureName(_culture);
        }

        /// <inheritdoc/>
        public string? GetName(IPublishedContent content)
        {
            return content.Name(_variationContextAccessor, _culture);
        }

        /// <inheritdoc/>
        public string? GetValue(string propertyAlias)
        {
            return _content.GetValue<string>(propertyAlias, culture: _culture).DefaultIfNullOrWhiteSpace(null);
        }

        /// <inheritdoc/>
        public string? GetValue(IPublishedContent content, string propertyAlias)
        {
            return content.Value<string>(_publishedValueFallback, propertyAlias, culture: _culture).DefaultIfNullOrWhiteSpace(null);
        }

        /// <inheritdoc/>
        public string? GetCulture()
        {
            return _culture;
        }
    }
}
