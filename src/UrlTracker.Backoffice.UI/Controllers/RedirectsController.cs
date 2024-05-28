using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    /// <summary>
    /// A controller for managing redirects for the URL Tracker
    /// </summary>
    [ApiController]
    [PluginController(Defaults.Routing.Area)]
    [Route(Defaults.Routing.Route)]
    internal class RedirectsController : UmbracoAuthorizedApiController
    {
        private readonly IRedirectRequestHandler _redirectRequestHandler;

        public RedirectsController(IRedirectRequestHandler redirectRequestHandler)
        {
            _redirectRequestHandler = redirectRequestHandler;
        }

        [HttpGet]
        [Produces(typeof(RedirectCollectionResponse))]
        public async Task<IActionResult> ListAsync([FromQuery] ListRedirectRequest request)
        {
            var model = await _redirectRequestHandler.GetAsync(request);

            return Ok(model);
        }

        /// <summary>
        /// Get the redirect with this specific ID
        /// </summary>
        /// <param name="redirectId">The unique identifier of the requested redirect</param>
        /// <returns>A 200 OK result with a redirect or 404 NOT FOUND if no redirect with given id exists</returns>
        [HttpGet("{redirectId}")]
        [Produces(typeof(RedirectResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Get([FromRoute] int redirectId)
        {
            var model = _redirectRequestHandler.GetById(redirectId);
            if (model is null) return NotFound();

            return Ok(model);
        }

        /// <summary>
        /// Create a new redirect
        /// </summary>
        /// <param name="request">The redirect to create</param>
        /// <returns>201 CREATED with the new redirect as body if the creation was successful or 400 BAD REQUEST if the request was invalid</returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        [Produces(typeof(RedirectResponse))]
        public IActionResult Create([FromBody] CreateRedirectRequest request)
        {
            var model = _redirectRequestHandler.Create(request);
            if (model is null) return NotFound();

            return Ok(model);
        }

        /// <summary>
        /// Change the redirect with this specific ID
        /// </summary>
        /// <param name="redirectId">The unique identifier of the requested redirect</param>
        /// <param name="request">The new properties of the redirect</param>
        /// <returns>200 OK with the new redirect as body if the update was successful, 404 NOT FOUND if no redirect with given id exists or 400 BAD REQUEST if the request was invalid</returns>
        [HttpPost]
        [Route("{redirectId}")]
        [Produces(typeof(RedirectResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Update([FromRoute] int redirectId, [FromBody] RedirectRequest request)
        {
            var model = _redirectRequestHandler.Update(redirectId, request);
            if (model is null) return NotFound();

            return Ok(model);
        }

        [HttpPost("updatebulk")]
        [Produces(typeof(IEnumerable<RedirectResponse>))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult UpdateBulk([FromBody] RedirectBulkRequest[] request)
        {
            var model = _redirectRequestHandler.UpdateBulk(request);
            if (model is null) return NotFound();

            return Ok(model);
        }

        /// <summary>
        /// Delete the redirect with this specific ID
        /// </summary>
        /// <param name="redirectId">The unique identifier of the requested redirect</param>
        /// <returns>204 NO CONTENT if the redirect was deleted successfully or 404 NOT FOUND if no redirect with given id exists</returns>
        [HttpPost]
        [Route("{redirectId}/delete")]
        [Produces(typeof(RedirectResponse))]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int redirectId)
        {
            var model = _redirectRequestHandler.Delete(redirectId);
            if (model is null) return NotFound();

            return Ok(model);
        }

        [HttpPost("deletebulk")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult DeleteBulk([FromBody] int[] ids)
        {
            var existingRecords = _redirectRequestHandler.Get(ids);
            if (ids.Any(x => !existingRecords.Select(x => x.Id).Contains(x)))
            {
                return NotFound();
            }
            _redirectRequestHandler.DeleteBulk(ids);

            return NoContent();
        }
    }
}
