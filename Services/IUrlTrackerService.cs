using System.Collections.Generic;
using InfoCaster.Umbraco.UrlTracker.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public interface IUrlTrackerService
	{
		#region Create

		bool AddEntry(UrlTrackerModel entry);
		bool AddRedirect(IContent newContent, IPublishedContent oldContent, UrlTrackerRedirectType redirectType, UrlTrackerReason reason, bool isChild = false);

		#endregion

		#region Get

		UrlTrackerModel GetEntryById(int id);
		UrlTrackerGetResult GetRedirects(int skip, int amount);
		UrlTrackerGetResult GetNotFounds(int skip, int amount);
		UrlTrackerGetResult GetRedirectsByFilter(int skip, int amount, UrlTrackerSortType sortType = UrlTrackerSortType.CreatedDesc, string searchQuery = "");
		List<UrlTrackerModel> GetForcedRedirects();
		bool RedirectExist(int redirectNodeId, string oldUrl);
		List<UrlTrackerDomain> GetDomains();
		string GetUrlByNodeId(int nodeId);
		IPublishedContent GetNodeById(int nodeId);

		#endregion

		#region Update

		void UpdateEntry(UrlTrackerModel entry);
		void Convert410To301ByNodeId(int nodeId);
		void ClearDomains();
		List<UrlTrackerModel> ReloadForcedRedirectsCache();

		#endregion

		#region Delete

		void DeleteEntryById(int id);
		void DeleteEntryByRedirectNodeId(int nodeId);

		#endregion
	}
}