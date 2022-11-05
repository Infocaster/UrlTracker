using System;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;

namespace UrlTracker.Backoffice.UI.Controllers
{
    /// <summary>
    /// A controller for managing redirects for the URL Tracker
    /// </summary>
    [PluginController(Defaults.Routing.Area)]
    public class RedirectsController : UmbracoAuthorizedApiController
    {
        /// <summary>
        /// Get the redirect with this specific ID
        /// </summary>
        /// <param name="id">The unique identifier of the requested redirect</param>
        /// <returns>A 200 OK result with a redirect or 404 NOT FOUND if no redirect with given id exists</returns>
        [HttpGet]
        public IActionResult Get([FromRoute] int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change the redirect with this specific ID
        /// </summary>
        /// <param name="id">The unique identifier of the requested redirect</param>
        /// <param name="request">The new properties of the redirect</param>
        /// <returns>200 OK with the new redirect as body if the update was successful, 404 NOT FOUND if no redirect with given id exists or 400 BAD REQUEST if the request was invalid</returns>
        [HttpPut]
        public IActionResult Update([FromRoute] int id, [FromBody] RedirectRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the redirect with this specific ID
        /// </summary>
        /// <param name="id">The unique identifier of the requested redirect</param>
        /// <returns>204 NO CONTENT if the redirect was deleted successfully or 404 NOT FOUND if no redirect with given id exists</returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        public IActionResult Delete([FromRoute] int id)
        {
            throw new NotImplementedException();
        }
    }
}
