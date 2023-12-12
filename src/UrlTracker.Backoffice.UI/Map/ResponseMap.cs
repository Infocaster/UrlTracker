using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Modules.Options;

namespace UrlTracker.Backoffice.UI.Map
{
    internal class ResponseMap
        : IMapDefinition
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactoryAbstraction;

        public ResponseMap(ILocalizationService localizationService, IUmbracoContextFactoryAbstraction umbracoContextFactoryAbstraction)
        {
            _localizationService = localizationService;
            _umbracoContextFactoryAbstraction = umbracoContextFactoryAbstraction;
        }

        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<UrlTrackerLegacyOptions, GetSettingsResponse>(
                (source, context) => new GetSettingsResponse(),
                Map);

            mapper.Define<Domain, GetLanguagesFromNodeResponseLanguage>(
                (source, context) =>
                {
                    var language = _localizationService.GetLanguageByIsoCode(source.LanguageIsoCode)!;
                    return new GetLanguagesFromNodeResponseLanguage(source.LanguageIsoCode, language.CultureName);
                },
                Map);

            mapper.Define<Redirect, RedirectViewModel>(
                (source, context) => new RedirectViewModel(),
                Map);

            mapper.Define<RedirectCollection, GetRedirectsResponse>(
                (source, context) => new GetRedirectsResponse(context.MapEnumerable<Redirect, RedirectViewModel>(source), source.Total));

            mapper.Define<ClientError, RedirectViewModel>(
                (source, context) => new RedirectViewModel(),
                Map);

            mapper.Define<ClientErrorCollection, GetNotFoundsResponse>(
                (source, context) => new GetNotFoundsResponse(context.MapEnumerable<ClientError, RedirectViewModel>(source), source.Total));
        }

        private static void Map(ClientError source, RedirectViewModel target, MapperContext context)
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

            target.OldUrlWithoutQuery = queryStart != -1 ? source.Url[..queryStart] : source.Url;
            target.RedirectHttpCode = 0;
            target.RedirectNodeId = null;
            target.RedirectPassThroughQueryString = false;
            target.RedirectRootNodeId = -1;
            target.RedirectUrl = null;
            target.Referrer = source.MostCommonReferrer;
            target.Remove404 = false;
        }

        private void Map(Redirect source, RedirectViewModel target, MapperContext context)
        {
            target.CalculatedRedirectUrl = source.TargetNode?.Url(_umbracoContextFactoryAbstraction, source.Culture) ?? source.TargetUrl;
            target.Culture = source.Culture;
            target.ForceRedirect = source.Force;
            target.Id = source.Id!.Value;
            target.Inserted = source.Inserted;
            target.Is404 = false;
            target.Notes = source.Notes;
            target.Occurrences = null;
            target.OldRegex = source.SourceRegex;
            target.OldUrl = source.SourceUrl;

            int queryIndex = target.OldUrl?.IndexOf("?") ?? -1;
            target.OldUrlWithoutQuery = queryIndex >= 0 ? target.OldUrl![..queryIndex] : target.OldUrl;
            target.RedirectHttpCode = (int)source.TargetStatusCode;
            target.RedirectNodeId = source.TargetNode?.Id;
            target.RedirectPassThroughQueryString = source.RetainQuery;
            target.RedirectRootNodeId = source.TargetRootNode?.Id ?? -1;
            target.RedirectUrl = source.TargetUrl;
            target.Referrer = null;
            target.Remove404 = false;
        }

        private void Map(Domain source, GetLanguagesFromNodeResponseLanguage target, MapperContext context)
        {
            var language = _localizationService.GetLanguageByIsoCode(source.LanguageIsoCode)!;
            target.Id = language.Id;
        }

        private static void Map(UrlTrackerLegacyOptions source, GetSettingsResponse target, MapperContext context)
        {
            target.AppendPortNumber = source.AppendPortNumber;
            target.EnableLogging = source.EnableLogging;
            target.IsDisabled = source.IsDisabled;
            target.IsNotFoundTrackingDisabled = source.IsNotFoundTrackingDisabled;
            target.TrackingDisabled = source.TrackingDisabled;
            target.BlockedUrlsList = source.BlockedUrlsList;
            target.AllowedUserAgents = source.AllowedUserAgents;
        }
    }
}
