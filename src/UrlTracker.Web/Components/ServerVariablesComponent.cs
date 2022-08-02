using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;
using UrlTracker.Web.Controllers;

namespace UrlTracker.Web.Components
{
    public class ServerVariablesComponent : IComponent
    {
        public void Initialize()
        {
            ServerVariablesParser.Parsing += OnParsing;
        }

        public void Terminate()
        {
            ServerVariablesParser.Parsing -= OnParsing;
        }

        private void OnParsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null) throw new InvalidOperationException("HttpContext is null.");
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            Dictionary<string, string> urlTrackerVariables = new Dictionary<string, string>()
            {
                { "base", urlHelper.GetUmbracoApiServiceBaseUrl<UrlTrackerManagerController>(controller => controller.GetSettings()) },
                { "deleteEntry", nameof(UrlTrackerManagerController.DeleteEntry) },
                { "getLanguagesOutNodeDomains", nameof(UrlTrackerManagerController.GetLanguagesOutNodeDomains) },
                { "getNodesWithDomains", nameof(UrlTrackerManagerController.GetNodesWithDomains) },
                { "getSettings", nameof(UrlTrackerManagerController.GetSettings) },
                { "addRedirect", nameof(UrlTrackerManagerController.AddRedirect) },
                { "updateRedirect", nameof(UrlTrackerManagerController.UpdateRedirect) },
                { "getRedirects", nameof(UrlTrackerManagerController.GetRedirects) },
                { "getNotFounds", nameof(UrlTrackerManagerController.GetNotFounds) },
                { "countNotFoundsThisWeek", nameof(UrlTrackerManagerController.CountNotFoundsThisWeek) },
                { "addIgnore404", nameof(UrlTrackerManagerController.AddIgnore404) },
                { "importRedirects", nameof(UrlTrackerManagerController.ImportRedirects) },
                { "exportRedirects", nameof(UrlTrackerManagerController.ExportRedirects) }
            };

            e.Add("urlTracker", urlTrackerVariables);
        }
    }
}
