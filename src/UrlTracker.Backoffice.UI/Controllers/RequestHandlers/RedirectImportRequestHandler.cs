using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using NUglify;
using Org.BouncyCastle.Asn1.X509;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectImport;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRedirectImportRequestHandler
    {
        Task<Stream> ExportAsLegacyCSVAsync();
        Task<Stream> ExportExampleLegacyCSVAsync();
        Task<int> ImportCSVAsync(ImportRedirectRequest request);
    }

    internal class RedirectImportRequestHandler : IRedirectImportRequestHandler
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IRedirectService _redirectService;
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactoryAbstraction;

        public RedirectImportRequestHandler(
            IScopeProvider scopeProvider,
            IRedirectService redirectService,
            IUmbracoContextFactoryAbstraction umbracoContextFactoryAbstraction)
        {
            _scopeProvider = scopeProvider;
            _redirectService = redirectService;
            _umbracoContextFactoryAbstraction = umbracoContextFactoryAbstraction;
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
                    var redirect = CreateRedirectFromCsv(record);
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
            return await ExportRedirectsAsLegacyCSVAsync(redirects);
        }

        public Task<Stream> ExportExampleLegacyCSVAsync()
        {
            using var cref = _umbracoContextFactoryAbstraction.EnsureUmbracoContext();

            var exampleContent = cref.GetContentAtRoot().FirstOrDefault();
            var cultureKvp = exampleContent?.Cultures.FirstOrDefault();
            var culture = cultureKvp.HasValue ? cultureKvp.Value.Key : null;

            // We are hardcoding redirects here, because they serve as examples.
            //    They show how a correctly formatted redirect might look.
            return ExportRedirectsAsLegacyCSVAsync(new[]
            {
                new Redirect
                {
                    Force = true,
                    Id = 1,
                    Inserted = DateTime.UtcNow.Date,
                    Key = Guid.NewGuid(),
                    Permanent = true,
                    RetainQuery = true,
                    Source = new UrlSourceStrategy("https://example.com/lorem/ipsum"),
                    Target = new ContentPageTargetStrategy(exampleContent, culture),
                },
                new Redirect
                {
                    Force = false,
                    Id = 2,
                    Inserted = DateTime.UtcNow.Date,
                    Key = Guid.NewGuid(),
                    Permanent = false,
                    RetainQuery = false,
                    Source = new RegexSourceStrategy("^[0-9]+$"),
                    Target = new UrlTargetStrategy("https://example.com/lorem/ipsum")
                }
            });
        }

        private async Task<Stream> ExportRedirectsAsLegacyCSVAsync(IEnumerable<Redirect> redirects)
        {
            var csvRedirects = redirects.Select(CreateCsvRow);

            MemoryStream result = new();
            using StreamWriter sw = new(result, leaveOpen: true);
            using CsvWriter cw = new(sw, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", NewLine = Environment.NewLine });

            cw.WriteHeader<CsvRedirect>();
            await cw.NextRecordAsync().ConfigureAwait(false);
            await cw.WriteRecordsAsync<CsvRedirect>(csvRedirects).ConfigureAwait(false);

            await cw.FlushAsync().ConfigureAwait(false);
            string? csvContent = sw.ToString();

            result.Position = 0;

            return result;
        }

        private CsvRedirect CreateCsvRow(Redirect redirect)
        {
            /* NOTE: export does not support redirects by extensions
             * 
             * This mapping device only supports exports using core source and target types.
             * That's because the import and export functions are not to be changed in the initial update.
             * This mapping logic needs to be re-evaluated once the new export feature is built.
             */
            var result = new CsvRedirect
            {
                Force = redirect.Force,
                PassThroughQueryString = redirect.RetainQuery,
                TargetStatusCode = redirect.Permanent ? StatusCodes.Status301MovedPermanently : StatusCodes.Status302Found
            };

            switch (redirect.Source)
            {
                case UrlSourceStrategy strategy: result.SourceUrl = strategy.Value; break;
                case RegexSourceStrategy strategy: result.SourceRegex = strategy.Value; break;
            }

            /* NOTE: sqewed translations
             * Content redirects no longer rely on a root node. The root node will therefore never be populated.
             * This is only for backwards compatibility with the old import/export model
             * 
             * ALSO NOTE: it is possible that the content no longer exists.
             * rows that are generated by such redirects will cause errors during import.
             * The mapper is not responsible for what makes it into the file
             */
            switch (redirect.Target)
            {
                case UrlTargetStrategy strategy: result.TargetUrl = strategy.Url; break;
                case ContentPageTargetStrategy strategy: result.TargetNodeId = strategy.Content?.Id; result.Culture = strategy.Culture; break;
                case MediaTargetStrategy strategy: result.TargetNodeId = strategy.Content?.Id; break;
            }

            return result;
        }

        private Redirect CreateRedirectFromCsv(CsvRedirect csvRedirect)
        {
            var result = new Redirect
            {
                Force = csvRedirect.Force,
                RetainQuery = csvRedirect.PassThroughQueryString,
                Permanent = csvRedirect.TargetStatusCode == StatusCodes.Status301MovedPermanently
            };

            if (!string.IsNullOrWhiteSpace(csvRedirect.SourceUrl))
            {
                result.Source = new UrlSourceStrategy(csvRedirect.SourceUrl);
            }

            if (!string.IsNullOrWhiteSpace(csvRedirect.SourceRegex))
            {
                result.Source = new RegexSourceStrategy(csvRedirect.SourceRegex);
            }

            if (csvRedirect.TargetNodeId.HasValue)
            {
                using var cref = _umbracoContextFactoryAbstraction.EnsureUmbracoContext();
                var targetcontent = cref.GetContentById(csvRedirect.TargetNodeId.Value);
                var targetmedia = cref.GetMediaById(csvRedirect.TargetNodeId.Value);

                if (targetcontent is not null)
                {
                    result.Target = new ContentPageTargetStrategy(targetcontent, csvRedirect.Culture);
                }
                else if (targetmedia is not null)
                {
                    result.Target = new MediaTargetStrategy(targetmedia);
                }
            }

            if (!string.IsNullOrWhiteSpace(csvRedirect.TargetUrl))
            {
                result.Target = new UrlTargetStrategy(csvRedirect.TargetUrl);
            }

            return result;
        }
    }
}
