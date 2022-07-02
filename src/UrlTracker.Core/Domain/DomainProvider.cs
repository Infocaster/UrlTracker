using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
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
        private readonly IOptions<UrlTrackerSettings> _options;

        public DomainProvider(IDomainService domainService,
                              IOptions<UrlTrackerSettings> options)
        {
            _domainService = domainService;
            _options = options;
        }

        public DomainCollection GetDomains()
        {
            var configurationValue = _options.Value;
            var domains = _domainService.GetAll(configurationValue.HasDomainOnChildNode);
            return CreateCollection(domains);
        }

        public DomainCollection GetDomains(int nodeId)
        {
            var configurationValue = _options.Value;
            var domains = _domainService.GetAssignedDomains(nodeId, configurationValue.HasDomainOnChildNode);
            return CreateCollection(domains);
        }

        private static DomainCollection CreateCollection(IEnumerable<IDomain> domains)
        {
            return DomainCollection.Create(from domain in domains
                                           let url = GetUrlFromDomain(domain)
                                           select new Models.Domain(domain.Id, domain.RootContentId, domain.DomainName, domain.LanguageIsoCode!.NormalizeCulture()!, url));
        }

        private static Url GetUrlFromDomain(IDomain domain)
        {
            return Url.Parse(domain.DomainName ?? throw new ArgumentException("The umbraco domain could not be interpreted.", nameof(domain)));
        }
    }
}
