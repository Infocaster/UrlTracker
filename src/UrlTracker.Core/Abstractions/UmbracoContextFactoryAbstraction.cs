using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace UrlTracker.Core.Abstractions
{
    [ExcludeFromCodeCoverage]
    internal class UmbracoContextFactoryAbstraction
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
    internal class UmbracoContextReferenceAbstraction
        : IUmbracoContextReferenceAbstraction
    {
        private readonly UmbracoContextReference _cref;
        private readonly IPublishedUrlProvider _urlProvider;

        public UmbracoContextReferenceAbstraction(UmbracoContextReference cref, IPublishedUrlProvider urlProvider)
        {
            _cref = cref;
            _urlProvider = urlProvider;
        }

        public virtual void Dispose()
        {
            _cref.Dispose();
            GC.SuppressFinalize(this);
        }

        public IPublishedContent? GetContentById(int id)
            => _cref.UmbracoContext.Content?.GetById(id);

        public string GetUrl(IPublishedContent content, UrlMode mode, string? culture)
            => content.Url(_urlProvider, culture, mode);

        public string GetMediaUrl(IPublishedContent content, UrlMode mode, string? culture)
            => content.MediaUrl(_urlProvider, culture, mode);

        public int? GetResponseCode()
        {
            return _cref.UmbracoContext.PublishedRequest?.ResponseStatusCode;
        }

        public IEnumerable<IPublishedContent> GetContentAtRoot()
        {
            return _cref.UmbracoContext.Content?.GetAtRoot() ?? Enumerable.Empty<IPublishedContent>();
        }

        public IPublishedContent? GetMediaById(int id)
        {
            return _cref.UmbracoContext.Media?.GetById(id);
        }
    }

    /// <summary>
    /// Extensions for <see cref="IUmbracoContextFactoryAbstraction"/>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class UmbracoContextFactoryAbstractionExtensions
    {
        /// <inheritdoc cref="IUmbracoContextReferenceAbstraction.GetUrl(IPublishedContent, UrlMode, string?)"/>
        /// <exception cref="NotSupportedException"></exception>
        public static string Url(this IPublishedContent content, IUmbracoContextFactoryAbstraction abstraction, string? culture = null, UrlMode mode = UrlMode.Default)
        {
            using var cref = abstraction.EnsureUmbracoContext();
            if (!content.HasCulture(culture)) culture = null;

            return content.ItemType switch
            {
                PublishedItemType.Content => cref.GetUrl(content, mode, culture),
                PublishedItemType.Media => cref.GetMediaUrl(content, mode, culture),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
