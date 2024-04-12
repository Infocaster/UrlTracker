using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.ActionFilters;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Core;

namespace UrlTracker.Backoffice.UI.Controllers
{
    // FIXME: This controller is for so many things at once. Separate into redirect, clienterror and dashboard or something
    [PluginController("urltracker")]
    [PatchModel]
    [Obsolete("This controller should no longer be used and should be phased out. Use a dedicated controller for dedicated features")]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Controller endpoints are routed by convention, so names cannot be changed without consequences")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    internal class UrlTrackerManagerController
        : UmbracoAuthorizedApiController
    {
        private readonly IRedirectService _redirectService;
        private readonly IClientErrorService _clientErrorService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoMapper _mapper;

        public UrlTrackerManagerController(IRedirectService redirectService,
                                           IClientErrorService clientErrorService,
                                           IScopeProvider scopeProvider,
                                           IUmbracoMapper mapper)
            : base()
        {
            _redirectService = redirectService;
            _clientErrorService = clientErrorService;
            _scopeProvider = scopeProvider;
            _mapper = mapper;
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> DeleteEntry([FromQuery] DeleteEntryRequest request)
        {
            var entry = await _clientErrorService.GetAsync(request.Id!.Value);
            if (entry is null) return NotFound();

            await _clientErrorService.DeleteAsync(entry);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetRedirects([FromQuery] GetRedirectsRequest request)
        {
            ExtractOrderParameters(request.SortType, out bool descending, out Core.Database.Models.OrderBy orderBy);

            var result = await _redirectService.GetAsync((uint)request.Skip!.Value, (uint)request.Amount!.Value, request.Query, descending);
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
            var clientError = await _clientErrorService.GetAsync(request.Id!.Value);
            if (clientError is null) return NotFound();

            clientError.Ignored = true;
            await _clientErrorService.UpdateAsync(clientError);
            return NoContent();
        }

        private static void ExtractOrderParameters(OrderBy sortType, out bool descending, out Core.Database.Models.OrderBy orderBy)
        {
            // Every even value in the enum type sorts by descending value, so sort by descending if value modulo 2 is 0
            //    There are 4 values, each descending and ascending. Divide by 2 maps exactly to the core enumerable.
            descending = (int)sortType % 2 == 0;
            orderBy = (Core.Database.Models.OrderBy)((int)sortType / 2);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
