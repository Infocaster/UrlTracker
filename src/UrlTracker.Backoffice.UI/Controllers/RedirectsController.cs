using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> List([FromQuery] ListRedirectRequest request)
        {
            var model = await _redirectRequestHandler.GetAsync(request);

            return Ok(model);
        }

        /// <summary>
        /// Get the redirect with this specific ID
        /// </summary>
        /// <param name="id">The unique identifier of the requested redirect</param>
        /// <returns>A 200 OK result with a redirect or 404 NOT FOUND if no redirect with given id exists</returns>
        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            var model = _redirectRequestHandler.GetById(id);
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
        public IActionResult Create([FromBody] RedirectRequest request)
        {
            var model = _redirectRequestHandler.Create(request);

            return Ok(model);
        }

        /// <summary>
        /// Change the redirect with this specific ID
        /// </summary>
        /// <param name="id">The unique identifier of the requested redirect</param>
        /// <param name="request">The new properties of the redirect</param>
        /// <returns>200 OK with the new redirect as body if the update was successful, 404 NOT FOUND if no redirect with given id exists or 400 BAD REQUEST if the request was invalid</returns>
        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] RedirectRequest request)
        {
            var model = _redirectRequestHandler.Update(id, request);
            if (model is null) return NotFound();

            return Ok(model);
        }

        [HttpPut]
        public IActionResult UpdateBulk([FromBody] RedirectBulkRequest[] request)
        {
            var model = _redirectRequestHandler.UpdateBulk(request);
            if (model is null) return NotFound();

            return Ok(model);
        }

        /// <summary>
        /// Delete the redirect with this specific ID
        /// </summary>
        /// <param name="id">The unique identifier of the requested redirect</param>
        /// <returns>204 NO CONTENT if the redirect was deleted successfully or 404 NOT FOUND if no redirect with given id exists</returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var model = _redirectRequestHandler.Delete(id);
            if (model is null) return NotFound();

            return Ok(model);
        }

        [HttpDelete]
        public IActionResult DeleteBulkAsync([FromBody] int[] ids)
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
