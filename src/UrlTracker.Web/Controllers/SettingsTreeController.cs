using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Formatting;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.WebApi.Filters;
using UrlTracker.Core;
namespace UrlTracker.Web.Controllers
{
    [Tree(Constants.Applications.Settings, Defaults.SettingsTree.UrlTrackerSettingsTreeName, TreeTitle = "UrlTracker settings", TreeGroup = "urlTrackerTreeGroup", SortOrder = 5)]
    [PluginController(Defaults.SettingsTree.UrlTrackerArea)]
    [ExcludeFromCodeCoverage]
    public class SettingsTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, [System.Web.Http.ModelBinding.ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return TreeNodeCollection.Empty;
        }

        protected override MenuItemCollection GetMenuForNode(string id, [System.Web.Http.ModelBinding.ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            // return a new (empty) MenuItemCollection since the tree has no child nodes to interact with
            return MenuItemCollection.Empty;
        }

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var rootResult = base.CreateRootNode(queryStrings);
            rootResult.RoutePath = string.Format("{0}/{1}/{2}", Constants.Applications.Settings, Defaults.SettingsTree.UrlTrackerSettingsTreeName, "overview"); ;
            rootResult.Icon = "icon-umb-developer";
            rootResult.HasChildren = false;
            rootResult.MenuUrl = null;
            return rootResult;
        }
    }
}