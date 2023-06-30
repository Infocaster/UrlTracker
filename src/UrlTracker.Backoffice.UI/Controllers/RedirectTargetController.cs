using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;
using UrlTracker.Backoffice.UI.Controllers.RequestHandlers;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [PluginController(Defaults.Routing.Area)]
    internal class RedirectTargetController : UmbracoAuthorizedApiController
    {
        private readonly IRedirectTargetRequestHandler _requestHandler;

        public RedirectTargetController(IRedirectTargetRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpGet]
        public IActionResult Content([FromQuery] GetContentTargetRequest request)
        {
            var model = _requestHandler.GetContentTarget(request);
            if (model is null) return NotFound();

            return Ok(model);
        }
    }
}
