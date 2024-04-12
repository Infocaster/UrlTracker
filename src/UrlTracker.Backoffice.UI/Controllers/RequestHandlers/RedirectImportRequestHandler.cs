using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectImport;
using UrlTracker.Core;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRedirectImportRequestHandler
    {
        Task<Stream> ExportAsLegacyCSVAsync();
        Task<int> ImportCSVAsync(ImportRedirectRequest request);
    }

    internal class RedirectImportRequestHandler : IRedirectImportRequestHandler
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IRedirectService _redirectService;
        private readonly IUmbracoMapper _mapper;

        public RedirectImportRequestHandler(
            IScopeProvider scopeProvider,
            IRedirectService redirectService,
            IUmbracoMapper mapper)
        {
            _scopeProvider = scopeProvider;
            _redirectService = redirectService;
            _mapper = mapper;
        }

        public async Task<int> ImportCSVAsync(ImportRedirectRequest request)
        {
            using StreamReader sr = new(request.Redirects.OpenReadStream());
            using CsvReader cr = new(sr, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

            // As preparation for later:
            //    Using the headers, we can determine which import strategy to use to import all records appropriately
            await cr.ReadAsync();
            cr.ReadHeader();
            var headerRecord = cr.HeaderRecord;

            var records = cr.GetRecordsAsync<CsvRedirect>();
            int amountOfRedirects = 0;

            using var scope = _scopeProvider.CreateScope();
            await foreach (var record in records)
            {
                try
                {
                    var redirect = _mapper.Map<CsvRedirect, Redirect>(record)!;
                    await _redirectService.AddAsync(redirect);

                    amountOfRedirects++;
                }
                catch (Exception e)
                {
                    // Enrich exception with a wrapper that explains which redirect caused the error
                    throw new InvalidOperationException($"An error occurred while importing redirect on line {amountOfRedirects + 2}. See inner exception for more details.", e);
                }
            }

            scope.Complete();

            return amountOfRedirects;
        }

        public async Task<Stream> ExportAsLegacyCSVAsync()
        {
            var redirects = await _redirectService.GetAsync();
            var csvRedirects = _mapper.MapEnumerable<Redirect, CsvRedirect>(redirects);
            string? csvContent;

            MemoryStream result = new();
            using StreamWriter sw = new(result, leaveOpen: true);
            using CsvWriter cw = new(sw, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", NewLine = Environment.NewLine });

            cw.WriteHeader<CsvRedirect>();
            await cw.NextRecordAsync().ConfigureAwait(false);
            await cw.WriteRecordsAsync<CsvRedirect>(csvRedirects).ConfigureAwait(false);

            await cw.FlushAsync().ConfigureAwait(false);
            csvContent = sw.ToString();

            result.Position = 0;

            return result;
        }
    }
}
