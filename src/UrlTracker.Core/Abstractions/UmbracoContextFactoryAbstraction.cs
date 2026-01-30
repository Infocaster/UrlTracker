using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure;
using Umbraco.Extensions;

namespace UrlTracker.Core.Abstractions
{
    [ExcludeFromCodeCoverage]
    internal class UmbracoContextFactoryAbstraction(
        IUmbracoContextFactory umbracoContextFactory,
        IPublishedUrlProvider urlProvider,
        IServiceScopeFactory serviceScopeFactory)
        : IUmbracoContextFactoryAbstraction
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory = umbracoContextFactory;
        private readonly IPublishedUrlProvider _urlProvider = urlProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public IUmbracoContextReferenceAbstraction EnsureUmbracoContext()
        {
            var serviceScope = _serviceScopeFactory.CreateScope();
            return new UmbracoContextReferenceAbstraction(_umbracoContextFactory.EnsureUmbracoContext(), _urlProvider, serviceScope);
        }
    }

    [ExcludeFromCodeCoverage]
    internal class UmbracoContextReferenceAbstraction
        : IUmbracoContextReferenceAbstraction
    {
        private readonly UmbracoContextReference _cref;
        private readonly IPublishedUrlProvider _urlProvider;
        private readonly IServiceScope _serviceScope;
        private readonly IPublishedContentQuery _publishedContentQuery;

        public UmbracoContextReferenceAbstraction(
            UmbracoContextReference cref,
            IPublishedUrlProvider urlProvider,
            IServiceScope serviceScope)
        {
            _cref = cref;
            _urlProvider = urlProvider;
            _serviceScope = serviceScope;
            _publishedContentQuery = _serviceScope.ServiceProvider.GetRequiredService<IPublishedContentQuery>();
        }

        public virtual void Dispose()
        {
            _cref.Dispose();
            _serviceScope.Dispose();
            GC.SuppressFinalize(this);
        }

        public IPublishedContent? GetContentById(int id)
            => _cref.UmbracoContext.Content?.GetById(id);

        public IPublishedContent? GetContentById(Guid key)
            => _cref.UmbracoContext.Content?.GetById(key);

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
            return _publishedContentQuery.ContentAtRoot();
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
