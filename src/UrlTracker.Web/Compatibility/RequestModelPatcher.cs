using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Compatibility
{
    [Obsolete("This service is temporarily introduced to remain compatible with the existing frontend. Do not use.")]
    [ExcludeFromCodeCoverage]
    public class RequestModelPatcher
        : IRequestModelPatcher
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IDomainProvider _domainProvider;

        public RequestModelPatcher(IUmbracoContextFactory umbracoContextFactory, IDomainProvider domainProvider)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _domainProvider = domainProvider;
        }

        public AddRedirectRequest Patch(AddRedirectRequest request)
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            if (request.RedirectNodeId.HasValue)
            {
                var content = cref.UmbracoContext.Content.GetById(request.RedirectNodeId.Value);
                if (content is not null)
                {
                    request.RedirectRootNodeId = content.Root().Id;
                    return request;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.OldUrl))
            {
                var url = Url.Parse(request.OldUrl);
                if (url.AvailableUrlTypes.Contains(UrlType.Absolute))
                {
                    var domains = _domainProvider.GetDomains();
                    var domain = domains.FirstOrDefault(d => d.Url.Host!.Equals(url.Host) && url.Path!.StartsWith(d.Url.Path!));
                    if (domain?.NodeId is not null)
                    {
                        request.RedirectRootNodeId = domain.NodeId.Value;
                        return request;
                    }
                }
            }

            throw new ArgumentException("Unable to patch this model");
        }
    }
}
