using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Composing;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
    public class UrlTrackerDomain
    {
        private IUrlTrackerService _urlTrackerService => Current.Factory.GetInstanceFor<IUrlTrackerService, UrlTrackerService>();
        private IUrlTrackerNewSettings _urlTrackerSettings => Current.Factory.GetInstanceFor<IUrlTrackerNewSettings, UrlTrackerNewSettings>();

        public int Id { get; set; }
        public int NodeId { get; set; }
        public string Name { get; set; }

		private IPublishedContent _node = null;
		public IPublishedContent Node
		{
			get
			{
				if (_node == null)
				{
					_node = _urlTrackerService.GetNodeById(NodeId);
				}

				return _node;
			}
		}
        public string UrlWithDomain
        {
            get
            {
                if (_urlTrackerSettings.HasDomainOnChildNode() && Node.Parent != null)
                {
                    using (InfoCaster.Umbraco.UrlTracker.Helpers.ContextHelper.EnsureHttpContext())
                    {
                        // not sure if this will ever occur because the ensurehttpcontext is now added...
                        if (Current.UmbracoContext != null)
                        {
                            /*var url = new Node(node.Id).Url;
                            return url;*/
                            return Node.Url; // do not re-instantiate
                        }
                        else
                        {
                            return string.Format("{0}{1}{2}", HttpContext.Current != null ? HttpContext.Current.Request.Url.Scheme : Uri.UriSchemeHttp, Uri.SchemeDelimiter, HttpContext.Current.Request.Url.Host + "/" + Node.Parent.Name + "/" + Node.Name);
                        }
                    }
                }
                else
                {
                    if (Name.Contains(Uri.UriSchemeHttp))
                    {
                        return Name;
                    }
                    else
                    {
                        return string.Format("{0}{1}{2}", HttpContext.Current != null ? HttpContext.Current.Request.Url.Scheme : Uri.UriSchemeHttp, Uri.SchemeDelimiter, Name);
                    }
                }
            }
        }

        public UrlTrackerDomain(int id, int nodeId, string name)
        {
            Id = id;
            NodeId = nodeId;
            Name = name;
        }

        public override string ToString()
        {
            return UrlWithDomain;
        }
    }
}