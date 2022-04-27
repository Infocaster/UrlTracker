using System.Net;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Map
{
    public class RequestMap
        : IMapDefinition
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public RequestMap(IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public void DefineMaps(UmbracoMapper mapper)
        {
            mapper.Define<RedirectRequestBase, Redirect>(
                (source, context) => new Redirect(),
                Map);

            mapper.Define<UpdateRedirectRequest, Redirect>(
                (source, context) => context.Map<RedirectRequestBase, Redirect>(source),
                Map);
        }

        private static void Map(UpdateRedirectRequest source, Redirect target, MapperContext context)
        {
            target.Id = source.Id;
        }

        private void Map(RedirectRequestBase source, Redirect target, MapperContext context)
        {
            target.Culture = source.Culture;
            target.Force = source.ForceRedirect;
            target.Notes = source.Notes;
            target.PassThroughQueryString = source.RedirectPassThroughQueryString;
            target.SourceRegex = source.OldRegex;
            target.SourceUrl = source.OldUrl;
            target.TargetStatusCode = (HttpStatusCode)source.RedirectHttpCode;
            target.TargetUrl = source.RedirectUrl;
            target.Inserted = source.Inserted;

            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                target.TargetNode = source.RedirectNodeId.HasValue ? cref.GetContentById(source.RedirectNodeId.Value) : null;
                target.TargetRootNode = cref.GetContentById(source.RedirectRootNodeId);
            }
        }
    }
}
