using Umbraco.Core.Mapping;
using Umbraco.Core.Services;
using Umbraco.Web;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Map
{
    public class ResponseMap
        : IMapDefinition
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactoryAbstraction;

        public ResponseMap(ILocalizationService localizationService, IUmbracoContextFactoryAbstraction umbracoContextFactoryAbstraction)
        {
            _localizationService = localizationService;
            _umbracoContextFactoryAbstraction = umbracoContextFactoryAbstraction;
        }

        public void DefineMaps(UmbracoMapper mapper)
        {
            mapper.Define<UrlTrackerSettings, GetSettingsResponse>(
                (source, context) => new GetSettingsResponse(),
                Map);

            mapper.Define<Domain, GetLanguagesFromNodeResponseLanguage>(
                (source, context) => new GetLanguagesFromNodeResponseLanguage(),
                Map);

            mapper.Define<Redirect, RedirectViewModel>(
                (source, context) => new RedirectViewModel(),
                Map);

            mapper.Define<RedirectCollection, GetRedirectsResponse>(
                (source, context) => new GetRedirectsResponse(),
                Map);

            mapper.Define<RichNotFound, RedirectViewModel>(
                (source, context) => new RedirectViewModel(),
                Map);

            mapper.Define<RichNotFoundCollection, GetNotFoundsResponse>(
                (source, context) => new GetNotFoundsResponse(),
                Map);
        }

        private static void Map(RichNotFoundCollection source, GetNotFoundsResponse target, MapperContext context)
        {
            target.Entries = context.MapEnumerable<RichNotFound, RedirectViewModel>(source);
            target.NumberOfEntries = source.Total;
        }

        private static void Map(RichNotFound source, RedirectViewModel target, MapperContext context)
        {
            target.CalculatedRedirectUrl = null;
            target.Culture = null;
            target.ForceRedirect = false;
            target.Id = source.Id;
            target.Inserted = source.LatestOccurrence;
            target.Is404 = true;
            target.Notes = null;
            target.Occurrences = source.Occurrences;
            target.OldRegex = null;
            target.OldUrl = source.Url;

            var queryStart = source.Url.IndexOf('?');

            target.OldUrlWithoutQuery = queryStart != -1 ? source.Url.Substring(0, queryStart) : source.Url;
            target.RedirectHttpCode = 0;
            target.RedirectNodeId = null;
            target.RedirectPassThroughQueryString = false;
            target.RedirectRootNodeId = -1;
            target.RedirectUrl = null;
            target.Referrer = source.MostCommonReferrer;
            target.Remove404 = false;
        }

        private static void Map(RedirectCollection source, GetRedirectsResponse target, MapperContext context)
        {
            target.NumberOfEntries = source.Total;
            target.Entries = context.MapEnumerable<Redirect, RedirectViewModel>(source);
        }

        private void Map(Redirect source, RedirectViewModel target, MapperContext context)
        {
            target.CalculatedRedirectUrl = source.TargetNode?.Url(_umbracoContextFactoryAbstraction, culture: source.Culture) ?? source.TargetUrl;
            target.Culture = source.Culture;
            target.ForceRedirect = source.Force;
            target.Id = source.Id.Value;
            target.Inserted = source.Inserted;
            target.Is404 = false;
            target.Notes = source.Notes;
            target.Occurrences = null;
            target.OldRegex = source.SourceRegex;
            target.OldUrl = source.SourceUrl;

            int queryIndex = target.OldUrl?.IndexOf("?") ?? -1;
            target.OldUrlWithoutQuery = queryIndex >= 0 ? target.OldUrl.Substring(0, queryIndex) : target.OldUrl;
            target.RedirectHttpCode = ((int)source.TargetStatusCode);
            target.RedirectNodeId = source.TargetNode?.Id;
            target.RedirectPassThroughQueryString = source.PassThroughQueryString;
            target.RedirectRootNodeId = source.TargetRootNode?.Id ?? -1;
            target.RedirectUrl = source.TargetUrl;
            target.Referrer = null;
            target.Remove404 = false;
        }

        private void Map(Domain source, GetLanguagesFromNodeResponseLanguage target, MapperContext context)
        {
            var language = _localizationService.GetLanguageByIsoCode(source.LanguageIsoCode);
            target.Id = language.Id;
            target.IsoCode = source.LanguageIsoCode.ToLower();
            target.CultureName = language.CultureName;
        }

        private static void Map(UrlTrackerSettings source, GetSettingsResponse target, MapperContext context)
        {
            target.AppendPortNumber = source.AppendPortNumber;
            target.EnableLogging = source.LoggingEnabled;
            target.IsDisabled = source.IsDisabled;
            target.IsNotFoundTrackingDisabled = source.IsNotFoundTrackingDisabled;
            target.TrackingDisabled = source.IsTrackingDisabled;
        }
    }
}
