using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectImport;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [PluginController(Defaults.Routing.Area)]
    [Route(Defaults.Routing.Route)]
    internal class RedirectImportController : UmbracoAuthorizedApiController
    {
        private readonly IRedirectImportRequestHandler _requestHandler;

        public RedirectImportController(IRedirectImportRequestHandler requestHandler)
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
            if (!request.Redirects.ContentType.EndsWith("csv", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(request.Redirects), "File must be a CSV");
                return BadRequest(ModelState);
            }

            var result = await _requestHandler.ImportCSVAsync(request);
            return Ok(result);
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
