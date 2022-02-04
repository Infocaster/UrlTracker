using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    public class ServiceLayerMaps
        : IMapDefinition
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public ServiceLayerMaps(IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        [ExcludeFromCodeCoverage]
        public void DefineMaps(UmbracoMapper mapper)
        {
            mapper.Define<UrlTrackerShallowRedirect, ShallowRedirect>(
                (source, context) => new ShallowRedirect(),
                Map);

            mapper.Define<UrlTrackerRedirect, Redirect>(
                (source, context) => context.Map<UrlTrackerShallowRedirect, ShallowRedirect>(source, new Redirect()) as Redirect,
                Map);

            mapper.Define<UrlTrackerRedirectCollection, RedirectCollection>(
                (source, context) => RedirectCollection.Create(context.MapEnumerable<UrlTrackerRedirect, Redirect>(source), source.Total));

            mapper.Define<Redirect, UrlTrackerRedirect>(
                (source, context) => new UrlTrackerRedirect(),
                Map);

            mapper.Define<NotFound, UrlTrackerNotFound>(
                (source, context) => new UrlTrackerNotFound(),
                Map);

            mapper.Define<UrlTrackerNotFound, NotFound>(
                (source, context) => new NotFound(),
                Map);

            mapper.Define<UrlTrackerRichNotFoundCollection, RichNotFoundCollection>(
                (source, context) => RichNotFoundCollection.Create(context.MapEnumerable<UrlTrackerRichNotFound, RichNotFound>(source), source.Total));

            mapper.Define<UrlTrackerRichNotFound, RichNotFound>(
                (source, context) => new RichNotFound(),
                Map);
        }

        private static void Map(UrlTrackerRichNotFound source, RichNotFound target, MapperContext context)
        {
            target.Id = source.Id ?? 0;
            target.LatestOccurrence = source.LatestOccurrence;
            target.MostCommonReferrer = source.MostCommonReferrer;
            target.Url = source.Url;
            target.Occurrences = source.Occurrences;
        }

        private static void Map(UrlTrackerNotFound source, NotFound target, MapperContext context)
        {
            target.Id = source.Id;
            target.Inserted = source.Inserted;
            target.Referrer = source.Referrer;
            target.Url = source.Url;
            target.Ignored = source.Ignored;
        }

        private static void Map(NotFound source, UrlTrackerNotFound target, MapperContext context)
        {
            target.Id = source.Id;
            target.Inserted = source.Inserted;
            target.Referrer = source.Referrer;
            target.Url = source.Url;
            target.Ignored = source.Ignored;
        }

        private static void Map(Redirect source, UrlTrackerRedirect target, MapperContext context)
        {
            target.Culture = source.Culture;
            target.Force = source.Force;
            target.Id = source.Id;
            target.Inserted = source.Inserted;
            target.Notes = source.Notes;
            target.PassThroughQueryString = source.PassThroughQueryString;
            target.SourceRegex = source.SourceRegex;
            target.SourceUrl = source.SourceUrl;
            target.TargetNodeId = source.TargetNode?.Id;
            target.TargetRootNodeId = source.TargetRootNode?.Id;
            target.TargetStatusCode = source.TargetStatusCode;
            target.TargetUrl = source.TargetUrl;
        }

        private static void Map(UrlTrackerRedirect source, Redirect target, MapperContext context)
        {
            target.Inserted = source.Inserted;
            target.Notes = source.Notes;
        }

        private void Map(UrlTrackerShallowRedirect source, ShallowRedirect target, MapperContext context)
        {
            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                target.Culture = source.Culture;
                target.Force = source.Force;
                target.Id = source.Id;
                target.PassThroughQueryString = source.PassThroughQueryString;
                target.SourceRegex = source.SourceRegex;
                target.SourceUrl = source.SourceUrl;
                target.TargetNode = source.TargetNodeId.HasValue ? cref.GetContentById(source.TargetNodeId.Value) : null;
                target.TargetRootNode = source.TargetRootNodeId.HasValue ? cref.GetContentById(source.TargetRootNodeId.Value) : null;
                target.TargetStatusCode = source.TargetStatusCode;
                target.TargetUrl = source.TargetUrl;
            }
        }
    }
}
