using System;
using System.Net;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Map
{
    public class LegacyDatabaseMap
        : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<UrlTrackerEntry, UrlTrackerShallowRedirect>(
                (source, context) => new UrlTrackerShallowRedirect(),
                Map);

            mapper.Define<UrlTrackerEntry, UrlTrackerShallowClientError>(
                (source, context) => new UrlTrackerShallowClientError(GetStatusCode(source)),
                Map);

            mapper.Define<UrlTrackerEntry, UrlTrackerRedirect>(
                (source, context) => (UrlTrackerRedirect)context.Map<UrlTrackerEntry, UrlTrackerShallowRedirect>(source, new UrlTrackerRedirect()),
                Map);

            mapper.Define<UrlTrackerEntry, UrlTrackerNotFound>(
                (source, context) => new UrlTrackerNotFound(source.OldUrl!),
                Map);

            mapper.Define<UrlTrackerRedirect, UrlTrackerEntry>(
                (source, context) => new UrlTrackerEntry(),
                Map);

            mapper.Define<UrlTrackerNotFound, UrlTrackerEntry>(
                (source, context) => new UrlTrackerEntry(),
                Map);

            mapper.Define<UrlTrackerEntryNotFoundAggregate, UrlTrackerRichNotFound>(
                (source, context) => new UrlTrackerRichNotFound(source.OldUrl!),
                Map);
        }

        private static void Map(UrlTrackerEntryNotFoundAggregate source, UrlTrackerRichNotFound target, MapperContext context)
        {
            target.Id = source.Id;
            target.LatestOccurrence = source.Inserted;
            target.MostCommonReferrer = source.Referrer;
            target.Occurrences = source.Occurrences;
        }

        private static void Map(UrlTrackerEntry source, UrlTrackerNotFound target, MapperContext context)
        {
            target.Id = source.Id;
            target.Inserted = source.Inserted;
            target.Referrer = source.Referrer;
            target.Ignored = false; // ignored can be set to false, because if a 404 is ignored, it is completely removed from this table
        }

        private static void Map(UrlTrackerNotFound source, UrlTrackerEntry target, MapperContext context)
        {
            target.Culture = null;
            target.ForceRedirect = false;
            target.Id = source.Id ?? 0;
            target.Inserted = source.Inserted;
            target.Is404 = true;
            target.Notes = null;
            target.OldRegex = null;
            target.OldUrl = source.Url;
            target.RedirectHttpCode = 0;
            target.RedirectNodeId = null;
            target.RedirectPassThroughQueryString = false;
            target.RedirectRootNodeId = -1;
            target.RedirectUrl = null;
            target.Referrer = source.Referrer;
        }

        private static void Map(UrlTrackerRedirect source, UrlTrackerEntry target, MapperContext context)
        {
            target.Culture = source.Culture;
            target.ForceRedirect = source.Force;
            target.Id = source.Id ?? 0;
            target.Inserted = source.Inserted;
            target.Is404 = false;
            target.Notes = source.Notes;
            target.OldRegex = source.SourceRegex;
            target.OldUrl = source.SourceUrl;
            target.RedirectHttpCode = (int)source.TargetStatusCode;
            target.RedirectNodeId = source.TargetNodeId;
            target.RedirectPassThroughQueryString = source.PassThroughQueryString;
            target.RedirectRootNodeId = source.TargetRootNodeId;
            target.RedirectUrl = source.TargetUrl;
            target.Referrer = null;
        }

        private static void Map(UrlTrackerEntry source, UrlTrackerRedirect target, MapperContext context)
        {
            target.Inserted = source.Inserted;
            target.Notes = source.Notes;
        }

        private static void Map(UrlTrackerEntry source, UrlTrackerShallowClientError target, MapperContext context)
        {
            target.Id = source.Id;
            target.TargetStatusCode = GetStatusCode(source);
        }

        private static HttpStatusCode GetStatusCode(UrlTrackerEntry source)
        {
            return source.Is404 ? HttpStatusCode.NotFound : (HttpStatusCode)(source.RedirectHttpCode ?? throw new ArgumentException("RedirectHttpCode cannot be null if Is404 is false", nameof(source)));
        }

        private static void Map(UrlTrackerEntry source, UrlTrackerShallowRedirect target, MapperContext context)
        {
            target.Culture = source.Culture;
            target.Force = source.ForceRedirect;
            target.Id = source.Id;
            target.PassThroughQueryString = source.RedirectPassThroughQueryString;
            target.SourceRegex = source.OldRegex;
            target.SourceUrl = source.OldUrl;
            target.TargetNodeId = source.RedirectNodeId;
            target.TargetRootNodeId = source.RedirectRootNodeId;
            target.TargetStatusCode = GetStatusCode(source);
            target.TargetUrl = source.RedirectUrl;
        }
    }
}
