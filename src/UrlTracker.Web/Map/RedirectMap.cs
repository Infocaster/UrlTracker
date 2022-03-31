using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Map
{
    public class RedirectMap
        : IMapDefinition
    {
        private readonly IOptions<UrlTrackerSettings> _configuration;
        private readonly IDomainProvider _domainProvider;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactoryAbstraction;

        public RedirectMap(IOptions<UrlTrackerSettings> configuration,
                           IDomainProvider domainProvider,
                           IUmbracoContextFactoryAbstraction umbracoContextFactoryAbstraction)
        {
            _configuration = configuration;
            _domainProvider = domainProvider;
            _umbracoContextFactoryAbstraction = umbracoContextFactoryAbstraction;
        }

        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<ShallowRedirect, Url>(
                (source, context) =>
                {
                    var httpContext = context.GetHttpContext();

                    var configurationValue = _configuration.Value;
                    Url? url = null;
                    var request = httpContext!.Request;
                    if (source.TargetNode is not null)
                    {
                        url = Url.Parse(source.TargetNode.Url(_umbracoContextFactoryAbstraction, culture: !string.IsNullOrEmpty(source.Culture) ? source.Culture : null, UrlMode.Absolute));

                        // logic taken from the old url tracker:
                        url.Port = request.Host.Port != 80 && configurationValue.AppendPortNumber ? request.Host.Port : (int?)null;

                        // also from the old url tracker:
                        if (request.Host.Host != url.Host && _domainProvider.GetDomains().Any(d => d.NodeId == source.TargetRootNode?.Id && d.Url.Host!.Equals(request.Host.Host)))
                        {
                            url.Host = request.Host.Host;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(source.TargetUrl))
                    {
                        url = Url.Parse(source.TargetUrl);
                    }
                    else
                    {
                        // this occurs when the redirect points to a node, but this node has been trashed.
                        // HACK: I am required to return a non-null value, while the output can actually be null. Use null terminator to pretend as if the null is not actually a null.
                        return null!;
                    }

                    // then make changes to the url, based on the configuration
                    // ensure that the url is absolute by defining the host and the protocol if they weren't present yet.
                    if (string.IsNullOrEmpty(url.Host)) url.Host = request.Host.Host;

                    if (!url.Protocol.HasValue) url.Protocol = (Protocol)Enum.Parse(typeof(Protocol), request.Scheme, true);

                    if (source.PassThroughQueryString) url.Query = httpContext.Request.QueryString.Value;

                    return url;
                });
        }
    }
}
