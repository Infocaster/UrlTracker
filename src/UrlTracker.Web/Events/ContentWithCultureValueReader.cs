using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using UrlTracker.Core;

namespace UrlTracker.Web.Events
{
    [ExcludeFromCodeCoverage]
    public class ContentWithCultureValueReader : IContentValueReader
    {
        private readonly IContent _content;
        private readonly string? _culture;

        public ContentWithCultureValueReader(IContent content, ContentCultureInfos? culture = null)
        {
            _content = content;
            _culture = culture?.Culture.NormalizeCulture();
        }

        public string? GetName()
        {
            return _content.GetCultureName(_culture);
        }

        public string? GetName(IPublishedContent content)
        {
            return content.Name(_culture);
        }

        public string? GetValue(string propertyAlias)
        {
            return _content.GetValue<string>(propertyAlias, culture: _culture);
        }

        public string? GetValue(IPublishedContent content, string propertyAlias)
        {
            return content.Value<string>(propertyAlias, culture: _culture);
        }

        public string? GetCulture()
        {
            return _culture;
        }
    }
}
