using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using UrlTracker.Core;

namespace UrlTracker.Web.Controllers
{
    [Tree(Constants.Applications.Settings, Defaults.SettingsTree.UrlTrackerSettingsTreeName, TreeTitle = "UrlTracker settings", TreeGroup = "urlTrackerTreeGroup", SortOrder = 5)]
    [PluginController(Defaults.SettingsTree.UrlTrackerArea)]
    [ExcludeFromCodeCoverage]
    public class SettingsTreeController : TreeController
    {
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;
        public SettingsTreeController(ILocalizedTextService localizedTextService,
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
            IMenuItemCollectionFactory menuItemCollectionFactory,
            IEventAggregator eventAggregator)
        : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            _menuItemCollectionFactory = menuItemCollectionFactory;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
        {
            return TreeNodeCollection.Empty;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
        {
            // return a new (empty) MenuItemCollection since the tree has no child nodes to interact with
            return _menuItemCollectionFactory.Create();
        }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var rootResult = base.CreateRootNode(queryStrings);
            if (rootResult.Result is not null)
            {
                return rootResult;
            }

            var root = rootResult.Value;
            root.RoutePath = string.Format("{0}/{1}/{2}", Constants.Applications.Settings, Defaults.SettingsTree.UrlTrackerSettingsTreeName, "overview"); ;
            root.Icon = "icon-umb-developer";
            root.HasChildren = false;
            root.MenuUrl = null;

            return root;
        }
    }
}
