using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.ModelBinders;

namespace UrlTracker.Backoffice.UI.Controllers
{
    [Tree(Constants.Applications.Settings, Defaults.Tree.Settings, TreeTitle = "UrlTracker settings", TreeGroup = "urlTrackerTreeGroup", SortOrder = 1)]
    [PluginController(Defaults.Routing.Area)]
    [ExcludeFromCodeCoverage]
    public class UrlTrackerSettingsTreeController : TreeController
    {
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;
        public UrlTrackerSettingsTreeController(ILocalizedTextService localizedTextService,
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
            IMenuItemCollectionFactory menuItemCollectionFactory,
            IEventAggregator eventAggregator)
        : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            _menuItemCollectionFactory = menuItemCollectionFactory;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            return TreeNodeCollection.Empty;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            // return a new (empty) MenuItemCollection since the tree has no child nodes to interact with
            return _menuItemCollectionFactory.Create();
        }

        protected override ActionResult<TreeNode?> CreateRootNode(FormCollection queryStrings)
        {
            var rootResult = base.CreateRootNode(queryStrings);
            if (rootResult.Result is not null)
            {
                return rootResult;
            }

            var root = rootResult.Value!;
            root.RoutePath = string.Format("{0}/{1}/{2}", Constants.Applications.Settings, Defaults.Tree.Settings, "overview");
            root.Icon = "icon-umb-developer";
            root.HasChildren = false;
            root.MenuUrl = null;

            return root;
        }
    }
}
