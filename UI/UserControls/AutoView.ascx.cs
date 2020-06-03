using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
//using umbraco.NodeFactory;

namespace InfoCaster.Umbraco.UrlTracker.UI.UserControls
{
    public partial class AutoView : System.Web.UI.UserControl, IUrlTrackerView
    {
        public UrlTrackerModel UrlTrackerModel { get; set; }
        private UmbracoHelper _umbracoHelper;

        public AutoView(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if(_umbracoHelper == null)
            {
                _umbracoHelper = (UmbracoHelper) Current.Factory.GetInstance(typeof(UmbracoHelper));
            }
            lnkOldUrl.ToolTip = UrlTrackerResources.OldUrlTestInfo;
            rbPermanent.Text = UrlTrackerResources.RedirectType301;
            rbTemporary.Text = UrlTrackerResources.RedirectType302;
            cbRedirectPassthroughQueryString.Text = UrlTrackerResources.PassthroughQueryStringLabel;
        }

        public void LoadView()
        {
            UrlTrackerDomain domain = null;
            IPublishedContent redirectRootNode = _umbracoHelper.Content(UrlTrackerModel.RedirectRootNodeId);

            List<UrlTrackerDomain> domains = _umbracoHelper.GetDomains();
            domain = domains.FirstOrDefault(x => x.NodeId == redirectRootNode.Id);
            if (domain == null)
                domain = new UrlTrackerDomain(-1, redirectRootNode.Id, HttpContext.Current.Request.Url.Host);
            if (!domains.Any())
                pnlRootNode.Visible = false;
            else
            {
                lnkRootNode.Text = string.Format("{0} ({1})", domain.Node.Name, domain.Name);
                lnkRootNode.ToolTip = UrlTrackerResources.SyncTree;
                lnkRootNode.NavigateUrl = string.Format("javascript:parent.UmbClientMgr.mainTree().syncTree('{0}', false);", redirectRootNode.Path);
            }

            lnkOldUrl.Text = string.Format("{0} <i class=\"icon-share\"></i>", UrlTrackerModel.CalculatedOldUrl);
            lnkOldUrl.NavigateUrl = UrlTrackerModel.CalculatedOldUrlWithDomain;
            var redirectNode = _umbracoHelper.Content(UrlTrackerModel.RedirectNodeId.Value);
            lnkRedirectNode.Text = redirectNode.Id > 0 ? redirectNode.Name : UrlTrackerResources.RedirectNodeUnpublished;
            lnkRedirectNode.ToolTip = UrlTrackerResources.SyncTree;
            lnkRedirectNode.NavigateUrl = string.Format("javascript:parent.UmbClientMgr.mainTree().syncTree('{0}', false);", redirectNode.Path);
            if (UrlTrackerModel.RedirectHttpCode == 301)
                rbPermanent.Checked = true;
            else if (UrlTrackerModel.RedirectHttpCode == 302)
                rbTemporary.Checked = true;
            cbRedirectPassthroughQueryString.Checked = UrlTrackerModel.RedirectPassThroughQueryString;
            lblNotes.Text = UrlTrackerModel.Notes;
            lblInserted.Text = UrlTrackerModel.Inserted.ToString();
        }

        public void Save()
        {
            UrlTrackerModel.RedirectHttpCode = rbPermanent.Checked ? 301 : 302;
            UrlTrackerModel.RedirectPassThroughQueryString = cbRedirectPassthroughQueryString.Checked;
            UrlTrackerRepository.UpdateUrlTrackerEntry(UrlTrackerModel);
        }
    }
}