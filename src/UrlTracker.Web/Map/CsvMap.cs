using System.Net;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Models;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Map
{
    public class CsvMap
        : IMapDefinition
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;

        public CsvMap(IUmbracoContextFactoryAbstraction umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<Redirect, CsvRedirect>(
                (source, context) => new CsvRedirect(),
                Map);

            mapper.Define<CsvRedirect, Redirect>(
                (source, context) => new Redirect(),
                Map);
        }

        private void Map(CsvRedirect source, Redirect target, MapperContext context)
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            target.Culture = source.Culture;
            target.Force = source.Force;
            target.Notes = source.Notes;
            target.RetainQuery = source.PassThroughQueryString;
            target.SourceRegex = source.SourceRegex;
            target.SourceUrl = source.SourceUrl;
            target.TargetNode = source.TargetNodeId.HasValue ? cref.GetContentById(source.TargetNodeId.Value) : null;
            target.TargetRootNode = source.TargetRootNodeId.HasValue ? cref.GetContentById(source.TargetRootNodeId.Value) : null;
            target.TargetStatusCode = (HttpStatusCode)source.TargetStatusCode;
            target.TargetUrl = source.TargetUrl;
        }

        private static void Map(Redirect source, CsvRedirect target, MapperContext context)
        {
            target.Culture = source.Culture;
            target.Force = source.Force;
            target.TargetNodeId = source.TargetNode?.Id;
            target.Notes = source.Notes;
            target.PassThroughQueryString = source.RetainQuery;
            target.SourceRegex = source.SourceRegex;
            target.TargetRootNodeId = source.TargetRootNode?.Id;
            target.SourceUrl = source.SourceUrl;
            target.TargetStatusCode = ((int)source.TargetStatusCode);
            target.TargetUrl = source.TargetUrl;
        }
    }
}
