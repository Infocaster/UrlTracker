using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectImport;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [VersionedApiBackOfficeRoute(Defaults.Routing.Area + "/RedirectImport")]
    [ApiExplorerSettings(GroupName = "Redirects")]
    [MapToApi(Defaults.Routing.SwaggerApi)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    internal class UrlTrackerRedirectImportController : ManagementApiControllerBase
    {
        private readonly IRedirectImportRequestHandler _requestHandler;

        public UrlTrackerRedirectImportController(IRedirectImportRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpPost("import")]
        [Produces(typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportAsync([FromForm] ImportRedirectRequest request)
        {
            /* ToDo: Controllers should not do validation. This should happen in model validation preferrably
             */
            if (!request.Redirects.ContentType.EndsWith("csv", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(request.Redirects.ContentType, "application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(request.Redirects), "File must be a CSV");
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _requestHandler.ImportCSVAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("export")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportAsync()
        {
            // NOTE: stream does not need to be disposed, because the file result will dispose of it automatically
            var fileStream = await _requestHandler.ExportAsLegacyCSVAsync();

            string filename = $"urltracker-redirects-{DateTime.UtcNow:yyyy-MM-dd}.csv";

            // set this header so that umbraco javascript understands how to name the file
            Response.Headers.Add("x-filename", filename);
            return File(fileStream, "text/csv", filename);
        }

        [HttpGet("exportexample")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExampleAsync()
        {
            // NOTE: stream does not need to be disposed, because the file result will dispose of it automatically
            var fileStream = await _requestHandler.ExportExampleLegacyCSVAsync();

            string filename = "example-redirect-import.csv";

            // set this header so that umbraco javascript understands how to name the file
            Response.Headers.Add("x-filename", filename);
            return File(fileStream, "text/csv", filename);
        }
    }
}
