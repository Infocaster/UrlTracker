using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UrlTracker.Core.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class UmbracoContextFactoryAbstraction
        : IUmbracoContextFactoryAbstraction
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public UmbracoContextFactoryAbstraction(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public IUmbracoContextReferenceAbstraction EnsureUmbracoContext()
        {
            return new UmbracoContextReferenceAbstraction(_umbracoContextFactory.EnsureUmbracoContext());
        }
    }

    [ExcludeFromCodeCoverage]
    public class UmbracoContextReferenceAbstraction
        : IUmbracoContextReferenceAbstraction
    {
        private readonly UmbracoContextReference _cref;

        public UmbracoContextReferenceAbstraction(UmbracoContextReference cref)
        {
            _cref = cref;
        }

        public virtual void Dispose()
        {
            _cref.Dispose();
            GC.SuppressFinalize(this);
        }

        public IPublishedContent GetContentById(int id)
            => _cref.UmbracoContext.Content?.GetById(id);

        public string GetUrl(IPublishedContent content, UrlMode mode, string culture)
            => _cref.UmbracoContext.UrlProvider.GetUrl(content, mode, culture);

        public string GetMediaUrl(IPublishedContent content, UrlMode mode, string culture)
            => _cref.UmbracoContext.UrlProvider.GetMediaUrl(content, mode, culture);

        public IEnumerable<IPublishedContent> GetContentAtRoot()
        {
            return _cref.UmbracoContext.Content?.GetAtRoot() ?? Enumerable.Empty<IPublishedContent>();
        }
    }

    [ExcludeFromCodeCoverage]
    public static class UmbracoContextFactoryAbstractionExtensions
    {
        public static string Url(this IPublishedContent content, IUmbracoContextFactoryAbstraction abstraction, string culture = null, UrlMode mode = UrlMode.Default)
        {
            using (var cref = abstraction.EnsureUmbracoContext())
            {
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
