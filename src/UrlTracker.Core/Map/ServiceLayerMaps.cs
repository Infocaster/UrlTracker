using System.Diagnostics.CodeAnalysis;
using System.Net;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models.Entities;
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
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<IRedirect, Redirect>(
                (source, context) => new Redirect(),
                Map);

            mapper.Define<Database.Entities.RedirectEntityCollection, Models.RedirectCollection>(
                (source, context) => Models.RedirectCollection.Create(context.MapEnumerable<IRedirect, Redirect>(source), source.Total));

            mapper.Define<Redirect, IRedirect>(
                (source, context) => new RedirectEntity(source.Culture, source.TargetRootNode?.Id, source.TargetNode?.Id, source.TargetUrl, source.SourceUrl, source.SourceRegex, source.RetainQuery, (int)source.TargetStatusCode == (int)HttpStatusCode.Moved, source.Force, source.Notes),
                Map);

            mapper.Define<ClientError, IClientError>(
                (source, context) => new ClientErrorEntity(source.Url, source.Ignored, source.Strategy),
                Map);

            mapper.Define<IClientError, ClientError>(
                (source, context) => new ClientError(source.Url),
                Map);

            mapper.Define<Database.Entities.ClientErrorEntityCollection, Models.ClientErrorCollection>(
                (source, context) => Models.ClientErrorCollection.Create(context.MapEnumerable<IClientError, ClientError>(source), source.Total));
        }

        private static void Map(IClientError source, ClientError target, MapperContext context)
        {
            target.Id = source.Id;
            target.LatestOccurrence = source.MostRecentOccurrence;
            target.MostCommonReferrer = source.MostCommonReferrer;
            target.Occurrences = source.TotalOccurrences;
            target.Ignored = source.Ignored;
            target.Inserted = source.CreateDate;
        }

        private static void Map(ClientError source, IClientError target, MapperContext context)
        {
            target.Id = source.Id;
            target.CreateDate = source.Inserted;
        }

        private static void Map(Redirect source, IRedirect target, MapperContext context)
        {
            target.CreateDate = source.Inserted;
            target.Id = source.Id ?? 0;
        }

        private void Map(IRedirect source, Redirect target, MapperContext context)
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();

            target.Inserted = source.CreateDate;
            target.Notes = source.Notes;
            target.Culture = source.Culture;
            target.Force = source.Force;
            target.Id = source.Id == 0 ? null : source.Id;
            target.RetainQuery = source.RetainQuery;
            target.SourceRegex = source.SourceRegex;
            target.SourceUrl = source.SourceUrl;
            target.TargetNode = source.TargetNodeId.HasValue ? cref.GetContentById(source.TargetNodeId.Value) : null;
            target.TargetRootNode = source.TargetRootNodeId.HasValue ? cref.GetContentById(source.TargetRootNodeId.Value) : null;
            target.TargetStatusCode = source.Permanent ? HttpStatusCode.Moved : HttpStatusCode.Redirect;
            target.TargetUrl = source.TargetUrl;
        }
    }
}
