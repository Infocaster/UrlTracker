using System.Collections.Generic;
using InfoCaster.Umbraco.UrlTracker.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public interface IUrlTrackerService
	{
		bool AddRedirect(IContent newContent, IPublishedContent oldContent, UrlTrackerRedirectType redirectType, UrlTrackerReason reason, bool isChild = false);
		List<UrlTrackerDomain> GetDomains();
		void ClearDomains();
		string GetUrlByNodeId(int nodeId);
		IPublishedContent GetNodeById(int nodeId);
	}
}