using System;
using System.Linq;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Map
{
    public class RedirectMap
        : IMapDefinition
    {
        private readonly IConfiguration<UrlTrackerSettings> _configuration;
        private readonly IDomainProvider _domainProvider;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactoryAbstraction;

        public RedirectMap(IConfiguration<UrlTrackerSettings> configuration,
                           IDomainProvider domainProvider,
                           IUmbracoContextFactoryAbstraction umbracoContextFactoryAbstraction)
        {
            _configuration = configuration;
            _domainProvider = domainProvider;
            _umbracoContextFactoryAbstraction = umbracoContextFactoryAbstraction;
        }

        public void DefineMaps(UmbracoMapper mapper)
        {
            mapper.Define<ShallowRedirect, Url>(
                (source, context) =>
                {
                    var httpContext = context.GetHttpContext();

                    var configurationValue = _configuration.Value;
                    Url url = null;
                    Uri requestUri = httpContext.Request.Url;
                    if (source.TargetNode != null)
                    {
                        url = Url.Parse(source.TargetNode.Url(_umbracoContextFactoryAbstraction, culture: !string.IsNullOrEmpty(source.Culture) ? source.Culture : null, UrlMode.Absolute));

                        // logic taken from the old url tracker:
                        url.Port = requestUri.Port != 80 && configurationValue.AppendPortNumber ? requestUri.Port : (int?)null;

                        // also from the old url tracker:
                        if (requestUri.Host != url.Host && _domainProvider.GetDomains().Any(d => d.NodeId == source.TargetRootNode?.Id && d.Url.Host.Equals(requestUri.Host)))
                        {
                            url.Host = requestUri.Host;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(source.TargetUrl))
                    {
                        url = Url.Parse(source.TargetUrl);
                    }
                    else
                    {
                        // this occurs when the redirect points to a node, but this node has been trashed.
                        return null;
                    }

                    // then make changes to the url, based on the configuration
                    // ensure that the url is absolute by defining the host and the protocol if they weren't present yet.
                    if (string.IsNullOrEmpty(url.Host)) url.Host = requestUri.Host;

                    if (!url.Protocol.HasValue) url.Protocol = (Protocol)Enum.Parse(typeof(Protocol), requestUri.Scheme, true);

                    if (source.PassThroughQueryString) url.Query = httpContext.Request.Url.Query;

                    return url;
                });
        }
    }
}
