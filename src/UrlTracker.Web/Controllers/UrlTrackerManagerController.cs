using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CsvHelper;
using CsvHelper.Configuration;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using UrlTracker.Core;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Web.Compatibility;
using UrlTracker.Web.Controllers.ActionFilters;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Controllers
{
    // FIXME: This controller is for so many things at once. Separate into redirect, notfound and dashboard or something
    [PluginController("urltracker")]
    [PatchModel]
    [ValidateModel]
    public class UrlTrackerManagerController
        : UmbracoAuthorizedApiController
    {
        private readonly IConfiguration<UrlTrackerSettings> _configuration;
        private readonly IDomainProvider _domainProvider;
        private readonly IRedirectService _redirectService;
        private readonly IClientErrorService _clientErrorService;
        private readonly ILegacyService _legacyService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IRequestModelPatcher _requestModelPatcher;

        public UrlTrackerManagerController(IGlobalSettings globalSettings,
                                           IUmbracoContextAccessor umbracoContextAccessor,
                                           ISqlContext sqlContext,
                                           ServiceContext services,
                                           AppCaches appCaches,
                                           IProfilingLogger logger,
                                           Umbraco.Core.IRuntimeState runtimeState,
                                           UmbracoHelper umbracoHelper,
                                           IConfiguration<UrlTrackerSettings> configuration,
                                           IDomainProvider domainProvider,
                                           IRedirectService redirectService,
                                           IClientErrorService notFoundService,
                                           ILegacyService legacyService,
                                           IScopeProvider scopeProvider,
                                           IRequestModelPatcher requestModelPatcher)
            : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
            _configuration = configuration;
            _domainProvider = domainProvider;
            _redirectService = redirectService;
            _clientErrorService = notFoundService;
            _legacyService = legacyService;
            _scopeProvider = scopeProvider;
            _requestModelPatcher = requestModelPatcher;
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IHttpActionResult> DeleteEntry([FromUri] DeleteEntryRequest request)
        {
            var entry = await _legacyService.GetAsync(request.Id.Value);
            if (entry is null) return NotFound();

            await _legacyService.DeleteAsync(entry);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public IHttpActionResult GetLanguagesOutNodeDomains([FromUri] GetLanguagesFromNodeRequest request)
        {
            // ToDo: Old implementation does not account for the possibility that a provided id might not map to an actual published content item
            //    Check for content item and return 404 if no content exists for a given id
            var domains = _domainProvider.GetDomains(request.NodeId.Value);
            var uniqueDomains = from domain in domains
                                group domain by domain.LanguageIsoCode into g
                                select g.First();

            // ToDo: This model is currently simply a list. This is not nice and can become difficult to work with
            //    Replace list with a real response model
            var model = Mapper.MapEnumerable<Domain, GetLanguagesFromNodeResponseLanguage>(uniqueDomains);
            return Ok(model);
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public IHttpActionResult GetSettings()
        {
            var result = _configuration.Value;
            var model = Mapper.Map<GetSettingsResponse>(result);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddRedirect([FromBody] AddRedirectRequest request)
        {
            // Wrap the operations inside a scope! If one of the operations fail, everything will be rolled back. This way, no data will ever get lost
            using (var scope = _scopeProvider.CreateScope())
            {
                Task deleteTask = null;
                if (request.Remove404)
                {
                    request = _requestModelPatcher.Patch(request);
                    deleteTask = _clientErrorService.DeleteAsync(request.OldUrl, request.Culture);
                }

                var redirect = Mapper.Map<Redirect>(request);
                await _redirectService.AddAsync(redirect);
                if (!(deleteTask is null)) await deleteTask;

                scope.Complete();
                return StatusCode(HttpStatusCode.Created);
            }
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IHttpActionResult> UpdateRedirect([FromBody] UpdateRedirectRequest request)
        {
            var redirect = Mapper.Map<Redirect>(request);
            await _redirectService.UpdateAsync(redirect);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetRedirects([FromUri] GetRedirectsRequest request)
        {
#pragma warning disable IDE0018 // Inline variable declaration
            bool descending;
            Core.Database.Models.OrderBy orderBy;
#pragma warning restore IDE0018 // Inline variable declaration
            ExtractOrderParameters(request.SortType, out descending, out orderBy);

            var result = await _redirectService.GetAsync((uint)request.Skip.Value, (uint)request.Amount.Value, request.Query, orderBy, descending);
            var model = Mapper.Map<GetRedirectsResponse>(result);

            return Ok(model);
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public async Task<IHttpActionResult> GetNotFounds([FromUri] GetNotFoundsRequest request)
        {
#pragma warning disable IDE0018 // Inline variable declaration
            bool descending;
            Core.Database.Models.OrderBy orderBy;
#pragma warning restore IDE0018 // Inline variable declaration
            ExtractOrderParameters(request.SortType, out descending, out orderBy);

            var result = await _clientErrorService.GetAsync((uint)request.Skip.Value, (uint)request.Amount.Value, request.Query, orderBy, descending);
            var model = Mapper.Map<GetNotFoundsResponse>(result);

            return Ok(model);
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public async Task<IHttpActionResult> CountNotFoundsThisWeek()
        {
            var now = DateTime.UtcNow;
            var result = await _clientErrorService.CountAsync(now.Date.AddDays(1 - (int)now.DayOfWeek), now);

            return Ok(result);
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IHttpActionResult> AddIgnore404([FromBody] int id)
        {
            var request = new AddIgnore404Request
            {
                Id = id
            };

            var notFound = await _clientErrorService.GetAsync(request.Id.Value);
            if (notFound is null) return NotFound();

            notFound.Ignored = true;
            await _clientErrorService.UpdateAsync(notFound);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IHttpActionResult> ImportRedirects()
        {
            /* ToDo: Controllers should not do validation. This should happen in model validation preferrably
             *    We can't do that in this case though, because file uploads don't work in web api
             *    Controllers should only deal with the outcome of validation. Is there a way to do that here as well?
             */
            var httpContextAttempt = TryGetHttpContext();
            if (!httpContextAttempt.Success) throw new Exception("Could not get http context");

            var httpContext = httpContextAttempt.Result;
            if (httpContext.Request.Files.Count < 1)
            {
                ModelState.AddModelError("file", "No files were uploaded");
                return BadRequest(ModelState);
            }
            var file = httpContext.Request.Files[0];
            if (!file.FileName.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("file", "Uploaded file does not have a valid extension");
                return BadRequest(ModelState);
            }

            using (StreamReader sr = new StreamReader(file.InputStream))
            {
                using (CsvReader cr = new CsvReader(sr, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    var records = cr.GetRecords<CsvRedirect>();
                    var redirects = Mapper.MapEnumerable<CsvRedirect, Redirect>(records);
                    using (var scope = _scopeProvider.CreateScope())
                    {
                        foreach (var redirect in redirects)
                        {
                            await _redirectService.AddAsync(redirect);
                        }

                        scope.Complete();
                    }

                    return Ok(redirects.Count);
                }
            }
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public async Task<HttpResponseMessage> ExportRedirects()
        {
            var redirects = await _redirectService.GetAsync();
            var csvRedirects = Mapper.MapEnumerable<Redirect, CsvRedirect>(redirects);
            string csvContent;
            using (StringWriter sw = new StringWriter())
            {
                using (CsvWriter cw = new CsvWriter(sw, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", NewLine = Environment.NewLine }))
                {
                    cw.WriteHeader<CsvRedirect>();
                    await cw.NextRecordAsync().ConfigureAwait(false);
                    await cw.WriteRecordsAsync<CsvRedirect>(csvRedirects).ConfigureAwait(false);

                    await cw.FlushAsync().ConfigureAwait(false);
                    csvContent = sw.ToString();
                }
            }

            var result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StringContent(csvContent, Encoding.UTF8, "text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = $"urltracker-redirects-{DateTime.UtcNow:yyyy-MM-dd}.csv"
            };

            return result;
        }

        private static void ExtractOrderParameters(OrderBy sortType, out bool descending, out Core.Database.Models.OrderBy orderBy)
        {
            // Every even value in the enum type sorts by descending value, so sort by descending if value modulo 2 is 0
            //    There are 4 values, each descending and ascending. Divide by 2 maps exactly to the core enumerable.
            descending = (((int)sortType) % 2) == 0;
            orderBy = (Core.Database.Models.OrderBy)(((int)sortType) / 2);
        }
    }
}
