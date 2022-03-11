using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace UrlTracker.Core.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class UmbracoContextFactoryAbstraction
        : IUmbracoContextFactoryAbstraction
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IPublishedUrlProvider _urlProvider;

        public UmbracoContextFactoryAbstraction(IUmbracoContextFactory umbracoContextFactory, IPublishedUrlProvider urlProvider)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _urlProvider = urlProvider;
        }

        public IUmbracoContextReferenceAbstraction EnsureUmbracoContext()
        {
            return new UmbracoContextReferenceAbstraction(_umbracoContextFactory.EnsureUmbracoContext(), _urlProvider);
        }
    }

    [ExcludeFromCodeCoverage]
    public class UmbracoContextReferenceAbstraction
        : IUmbracoContextReferenceAbstraction
    {
        private readonly UmbracoContextReference _cref;
        private readonly IPublishedUrlProvider _urlProvider;

        public UmbracoContextReferenceAbstraction(UmbracoContextReference cref, IPublishedUrlProvider urlProvider)
        {
            _cref = cref;
            _urlProvider = urlProvider;
        }

        public void Dispose()
        {
            _cref.Dispose();
        }

        public IPublishedContent GetContentById(int id)
            => _cref.UmbracoContext.Content.GetById(id);

        public string GetUrl(IPublishedContent content, UrlMode mode, string? culture)
            => content.Url(_urlProvider, culture, mode);

        public string GetMediaUrl(IPublishedContent content, UrlMode mode, string? culture)
            => content.MediaUrl(_urlProvider, culture, mode);

        public int? GetResponseCode()
        {
            return _cref.UmbracoContext.PublishedRequest.ResponseStatusCode;
        }
    }

    [ExcludeFromCodeCoverage]
    public static class UmbracoContextFactoryAbstractionExtensions
    {
        public static string Url(this IPublishedContent content, IUmbracoContextFactoryAbstraction abstraction, string? culture = null, UrlMode mode = UrlMode.Default)
        {
            using (var cref = abstraction.EnsureUmbracoContext())
            {
                if (!content.HasCulture(culture)) culture = null;

                switch (content.ItemType)
                {
                    case PublishedItemType.Content:
                        return cref.GetUrl(content, mode, culture);
                    case PublishedItemType.Media:
                        return cref.GetMediaUrl(content, mode, culture);
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
