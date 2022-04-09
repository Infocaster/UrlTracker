using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Core;
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
    public class UrlTrackerManagerController
        : UmbracoAuthorizedApiController
    {
        private readonly IOptions<UrlTrackerSettings> _configuration;
        private readonly IDomainProvider _domainProvider;
        private readonly IRedirectService _redirectService;
        private readonly IClientErrorService _clientErrorService;
        private readonly ILegacyService _legacyService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IRequestModelPatcher _requestModelPatcher;
        private readonly IUmbracoMapper _mapper;

        public UrlTrackerManagerController(IOptions<UrlTrackerSettings> configuration,
                                           IDomainProvider domainProvider,
                                           IRedirectService redirectService,
                                           IClientErrorService notFoundService,
                                           ILegacyService legacyService,
                                           IScopeProvider scopeProvider,
                                           IRequestModelPatcher requestModelPatcher,
                                           IUmbracoMapper mapper)
            : base()
        {
            _configuration = configuration;
            _domainProvider = domainProvider;
            _redirectService = redirectService;
            _clientErrorService = notFoundService;
            _legacyService = legacyService;
            _scopeProvider = scopeProvider;
            _requestModelPatcher = requestModelPatcher;
            _mapper = mapper;
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> DeleteEntry([FromQuery] DeleteEntryRequest request)
        {
            var entry = await _legacyService.GetAsync(request.Id!.Value);
            if (entry is null) return NotFound();

            await _legacyService.DeleteAsync(entry);
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetLanguagesOutNodeDomains([FromQuery] GetLanguagesFromNodeRequest request)
        {
            // ToDo: Old implementation does not account for the possibility that a provided id might not map to an actual published content item
            //    Check for content item and return 404 if no content exists for a given id
            var domains = _domainProvider.GetDomains(request.NodeId!.Value);
            var uniqueDomains = from domain in domains
                                group domain by domain.LanguageIsoCode into g
                                select g.First();

            // ToDo: This model is currently simply a list. This is not nice and can become difficult to work with
            //    Replace list with a real response model
            var model = _mapper.MapEnumerable<Domain, GetLanguagesFromNodeResponseLanguage>(uniqueDomains);
            return Ok(model);
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public IActionResult GetSettings()
        {
            var result = _configuration.Value;
            var model = _mapper.Map<GetSettingsResponse>(result);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRedirect([FromBody] AddRedirectRequest request)
        {
            // Wrap the operations inside a scope! If one of the operations fail, everything will be rolled back. This way, no data will ever get lost
            using var scope = _scopeProvider.CreateScope();
            Task? deleteTask = null;
            if (request.Remove404)
            {
                request = _requestModelPatcher.Patch(request);
                deleteTask = _clientErrorService.DeleteAsync(request.OldUrl!, request.Culture);
            }

            var redirect = _mapper.Map<Redirect>(request);
            await _redirectService.AddAsync(redirect);
            if (deleteTask is not null) await deleteTask;

            scope.Complete();
            return Ok();
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> UpdateRedirect([FromBody] UpdateRedirectRequest request)
        {
            var redirect = _mapper.Map<Redirect>(request);
            await _redirectService.UpdateAsync(redirect);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetRedirects([FromQuery] GetRedirectsRequest request)
        {
            ExtractOrderParameters(request.SortType, out bool descending, out Core.Database.Models.OrderBy orderBy);

            var result = await _redirectService.GetAsync((uint)request.Skip!.Value, (uint)request.Amount!.Value, request.Query, orderBy, descending);
            var model = _mapper.Map<GetRedirectsResponse>(result);

            return Ok(model);
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> GetNotFounds([FromQuery] GetNotFoundsRequest request)
        {
            ExtractOrderParameters(request.SortType, out bool descending, out Core.Database.Models.OrderBy orderBy);

            var result = await _clientErrorService.GetAsync((uint)request.Skip!.Value, (uint)request.Amount!.Value, request.Query, orderBy, descending);
            var model = _mapper.Map<GetNotFoundsResponse>(result);

            return Ok(model);
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> CountNotFoundsThisWeek()
        {
            var now = DateTime.UtcNow;
            var result = await _clientErrorService.CountAsync(now.Date.AddDays(1 - (int)now.DayOfWeek), now);

            return Ok(result);
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> AddIgnore404([FromBody] AddIgnore404Request request)
        {
            var notFound = await _clientErrorService.GetAsync(request.Id!.Value);
            if (notFound is null) return NotFound();

            await _clientErrorService.UpdateAsync(notFound);
            return NoContent();
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> ImportRedirects()
        {
            /* ToDo: Controllers should not do validation. This should happen in model validation preferrably
             *    We can't do that in this case though, because file uploads don't work in web api
             *    Controllers should only deal with the outcome of validation. Is there a way to do that here as well?
             */
            if (HttpContext.Request.Form.Files.Count < 1)
            {
                ModelState.AddModelError("file", "No files were uploaded");
                return BadRequest(ModelState);
            }
            var file = HttpContext.Request.Form.Files[0];
            if (!file.FileName.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("file", "Uploaded file does not have a valid extension");
                return BadRequest(ModelState);
            }

            using StreamReader sr = new(file.OpenReadStream());
            using CsvReader cr = new(sr, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

            var records = cr.GetRecords<CsvRedirect>();
            var redirects = _mapper.MapEnumerable<CsvRedirect, Redirect>(records);

            using var scope = _scopeProvider.CreateScope();
            foreach (var redirect in redirects)
            {
                await _redirectService.AddAsync(redirect);
            }

            scope.Complete();
            return Ok(redirects.Count);
        }

        [HttpGet]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> ExportRedirects()
        {
            var redirects = await _redirectService.GetAsync();
            var csvRedirects = _mapper.MapEnumerable<Redirect, CsvRedirect>(redirects);
            string? csvContent;
            using MemoryStream ms = new();
            using StreamWriter sw = new(ms);
            using (CsvWriter cw = new(sw, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", NewLine = Environment.NewLine }))
            {
                cw.WriteHeader<CsvRedirect>();
                await cw.NextRecordAsync().ConfigureAwait(false);
                await cw.WriteRecordsAsync<CsvRedirect>(csvRedirects).ConfigureAwait(false);

                await cw.FlushAsync().ConfigureAwait(false);
                csvContent = sw.ToString();
            }

            ms.Position = 0;

            return File(ms, "text/csv", $"urltracker-redirects-{DateTime.UtcNow:yyyy-MM-dd}.csv");
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
