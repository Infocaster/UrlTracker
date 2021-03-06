using System;
using System.Linq;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Database.Models;
using UrlTracker.Resources.Testing.Clients.Models;

namespace UrlTracker.Resources.Website.Maps
{
    public class PresetMaps : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<SeedRedirectRequestRedirect, UrlTrackerEntry>(
                (source, context) => new UrlTrackerEntry(),
                (source, target, context) =>
                {
                    target.Culture = source.Culture;
                    target.ForceRedirect = source.Force;
                    target.Id = source.Id ?? 0;
                    target.Is404 = false;
                    target.Notes = source.Notes;
                    target.OldRegex = source.SourceRegex;
                    target.OldUrl = source.SourceUrl;
                    target.RedirectHttpCode = source.TargetStatusCode;
                    target.RedirectNodeId = source.TargetNodeId;
                    target.RedirectPassThroughQueryString = source.PassThroughQueryString;
                    target.RedirectRootNodeId = source.TargetRootNodeId;
                    target.RedirectUrl = source.TargetUrl;
                    target.Referrer = string.Empty;
                    target.Inserted = DateTime.UtcNow;
                });

            mapper.Define<IPublishedContent, ContentTreeViewModel>(
                (source, context) => new ContentTreeViewModel(),
                (source, target, context) =>
                {
                    target.Name = source.Name;
                    target.Url = source.Url();
                    target.Id = source.Id;
                    target.Children = context.MapEnumerable<IPublishedContent, ContentTreeViewModel>(source.Children ?? Enumerable.Empty<IPublishedContent>());
                });
        }
    }
}