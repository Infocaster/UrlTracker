using System;
using System.Linq;
using System.Net;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Resources.Testing.Clients.Models;

namespace UrlTracker.Resources.Website.Maps
{
    public class PresetMaps : IMapDefinition
    {
        public void DefineMaps(UmbracoMapper mapper)
        {
            mapper.Define<SeedRedirectRequestRedirect, RedirectDto>(
                (source, context) => new RedirectDto(),
                (source, target, context) =>
                {
                    target.Culture = source.Culture;
                    target.Force = source.Force;
                    target.Id = source.Id ?? 0;
                    target.Notes = source.Notes;
                    target.SourceRegex = source.SourceRegex;
                    target.SourceUrl = source.SourceUrl;
                    target.Permanent = source.TargetStatusCode == (int)HttpStatusCode.MovedPermanently;
                    target.TargetNodeId = source.TargetNodeId;
                    target.RetainQuery = source.PassThroughQueryString;
                    target.TargetRootNodeId = source.TargetRootNodeId;
                    target.TargetUrl = source.TargetUrl;
                    target.CreateDate = DateTime.Now;
                    target.Key = Guid.NewGuid();
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