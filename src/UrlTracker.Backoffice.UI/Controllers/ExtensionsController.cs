using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Backoffice.UI.Controllers.Models.Extensions;
using UrlTracker.Backoffice.UI.Extensions;

namespace UrlTracker.Backoffice.UI.Controllers
{
    /// <summary>
    /// A controller for all extension points of the URL Tracker
    /// </summary>
    [PluginController(Defaults.Routing.Area)]
    public class ExtensionsController : UmbracoAuthorizedApiController
    {
        private readonly IUmbracoMapper _mapper;
        private readonly IUrlTrackerDashboardPageCollection _dashboardPageCollection;

        public ExtensionsController(IUmbracoMapper mapper, IUrlTrackerDashboardPageCollection dashboardPageCollection)
        {
            _mapper = mapper;
            _dashboardPageCollection = dashboardPageCollection;
        }

        /// <summary>
        /// Get all URL Tracker dashboard tabs
        /// </summary>
        /// <returns>All URL Tracker dashboard tabs</returns>
        public IActionResult DashboardPages()
        {
            var pages = _dashboardPageCollection.Get();
            var model = new DashboardPagesResponse(
                _mapper.MapEnumerable<IUrlTrackerDashboardPage, DashboardPagesResponsePage>(pages)
                );
            return Ok(model);
        }
    }
}
