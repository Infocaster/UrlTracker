using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectImport;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [ApiController]
    [ApiVersion(Defaults.Routing.V1.ApiVersion)]
    [MapToApi(Defaults.Routing.V1.ApiName)]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [Route(Defaults.Routing.V1.Route)]
    internal class RedirectImportController : Controller
    {
        private readonly IRedirectImportRequestHandler _requestHandler;

        public RedirectImportController(IRedirectImportRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpPost("import")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
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
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportAsync()
        {
            // NOTE: stream does not need to be disposed, because the file result will dispose of it automatically
            var fileStream = await _requestHandler.ExportAsLegacyCSVAsync();

            string filename = $"urltracker-redirects-{DateTime.UtcNow:yyyy-MM-dd}.csv";

            // set this header so that umbraco javascript understands how to name the file
            Response.Headers.Append("x-filename", filename);
            return File(fileStream, "text/csv", filename);
        }

        [HttpGet("exportexample")]
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExampleAsync()
        {
            // NOTE: stream does not need to be disposed, because the file result will dispose of it automatically
            var fileStream = await _requestHandler.ExportExampleLegacyCSVAsync();

            string filename = "example-redirect-import.csv";

            // set this header so that umbraco javascript understands how to name the file
            Response.Headers.Append("x-filename", filename);
            return File(fileStream, "text/csv", filename);
        }
    }
}
