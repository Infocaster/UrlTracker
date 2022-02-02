using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Domain
{
    // The domain provider separates the domain interpretation logic from the original service,
    //    because it's not the service's job to obtain the domains and it makes the service unnecessarily complicated
    // Caching is taken out and instead implemented as a decorator (see DecoratorDomainProviderCaching.cs)
    // Old code reference: https://dev.azure.com/infocaster/Umbraco%20Awesome/_git/UrlTracker?path=/Services/UrlTrackerService.cs&version=GBdevelop&line=322&lineEnd=347&lineStartColumn=3&lineEndColumn=4&lineStyle=plain&_a=contents
    [ExcludeFromCodeCoverage]
    internal class DomainProvider
        : IDomainProvider
    {
        private readonly IDomainService _domainService;
        private readonly IConfiguration<UrlTrackerSettings> _configuration;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public DomainProvider(IDomainService domainService,
                              IConfiguration<UrlTrackerSettings> configuration,
                              IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _domainService = domainService;
            _configuration = configuration;
            _umbracoContextFactory = umbracoContextFactory;
        }

        public DomainCollection GetDomains()
        {
            var configurationValue = _configuration.Value;
            var domains = _domainService.GetAll(configurationValue.HasDomainOnChildNode);
            return CreateCollection(configurationValue, domains);
        }

        public DomainCollection GetDomains(int nodeId)
        {
            var configurationValue = _configuration.Value;
            var domains = _domainService.GetAssignedDomains(nodeId, configurationValue.HasDomainOnChildNode);
            return CreateCollection(configurationValue, domains);
        }

        private DomainCollection CreateCollection(UrlTrackerSettings configurationValue, IEnumerable<IDomain> domains)
        {
            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                return DomainCollection.Create(from domain in domains
                                               let url = GetUrlFromDomain(domain, configurationValue, cref)
                                               select new Models.Domain(domain.Id, domain.RootContentId, domain.DomainName, domain.LanguageIsoCode, url));
            }
        }

        /*
         * The original model was "fat" and used a service locator. This is bad practice!!
         * The original model used these services to lazily compute the url
         *      Since the url is used straight away anyway, we might as well compute it immediately
         */
        private static Url GetUrlFromDomain(IDomain domain, UrlTrackerSettings settings, IUmbracoContextReferenceAbstraction cref)
        {
            // I'm taking some liberties on the original logic. The old logic ensured that the url starts with a protocol.
            //    I am parsing the url into an object to make comparisons with incoming requests easier and more straight forward
            string result = null;

            // This condition is taken from the old logic. I'm not sure why it works like this...
            if (settings.HasDomainOnChildNode && domain.RootContentId.HasValue)
            {
                var node = cref.GetContentById(domain.RootContentId.Value);
                if (node != null && node.Level > 1)
                {
                    result = node.Url(mode: UrlMode.Absolute);
                }
            }
            else
            {
                // otherwise read it immediately from the domain
                result = domain.DomainName;
            }

            return Url.Parse(result);
        }
    }
}
