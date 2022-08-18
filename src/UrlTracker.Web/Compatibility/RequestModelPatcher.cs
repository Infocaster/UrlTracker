using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
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
            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (request.RedirectNodeId.HasValue)
                {
                    var content = cref.UmbracoContext.Content?.GetById(request.RedirectNodeId.Value);
                    if (content != null)
                    {
                        request.RedirectRootNodeId = content.Root().Id;
                        return request;
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.OldUrl))
                {
                    var url = Url.Parse(request.OldUrl);

                    var domains = _domainProvider.GetDomains();
                    var domain = domains.FirstOrDefault(d => d.Url.ExtrapolatesTo(url));
                    if (domain?.NodeId != null)
                    {
                        request.RedirectRootNodeId = domain.NodeId.Value;
                        return request;
                    }

                    var rootContent = cref.UmbracoContext.Content.GetAtRoot(request.Culture);
                    foreach (var content in rootContent)
                    {
                        var contentUrl = Url.Parse(content.Url(request.Culture, UrlMode.Absolute));
                        if (contentUrl.ExtrapolatesTo(url))
                        {
                            request.RedirectRootNodeId = content.Id;
                            return request;
                        }
                    }

                    foreach (var content in rootContent)
                    {
                        var contentUrl = Url.Parse(content.Url(request.Culture, UrlMode.Relative));
                        if (contentUrl.ExtrapolatesTo(url))
                        {
                            request.RedirectRootNodeId = content.Id;
                            return request;
                        }
                    }
                }

                throw new ArgumentException("Unable to patch this model");
            }
        }
    }
}
