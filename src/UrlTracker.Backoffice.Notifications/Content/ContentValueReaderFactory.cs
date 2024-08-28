using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UrlTracker.Backoffice.Notifications.Content
{
    /// <summary>
    /// An implementation of <see cref="IContentValueReaderFactory" /> that produces content value readers based on content culture properties
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContentValueReaderFactory
        : IContentValueReaderFactory
    {
        private readonly IPublishedValueFallback _publishedValueFallback;
        private readonly IVariationContextAccessor _variationContextAccessor;

        /// <inheritdoc />
        public ContentValueReaderFactory(IPublishedValueFallback publishedValueFallback, IVariationContextAccessor variationContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            _variationContextAccessor = variationContextAccessor;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IContentValueReader> Create(IContent content, bool onlyChanged = false)
        {
            var result = new List<IContentValueReader>();

            if (content.ContentType.VariesByCulture())
            {
                foreach (var culture in content.CultureInfos!)
                {
                    if (!onlyChanged || culture.IsDirty())
                    {
                        result.Add(new ContentWithCultureValueReader(_publishedValueFallback, _variationContextAccessor, content, culture));
                    }
                }
            }
            else
            {
                result.Add(new ContentWithCultureValueReader(_publishedValueFallback, _variationContextAccessor, content, null));
            }

            return result;
        }
    }
}
