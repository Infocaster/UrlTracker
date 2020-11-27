using System.Collections.Generic;
using InfoCaster.Umbraco.UrlTracker.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public interface IUrlTrackerService
	{
		#region Create

		bool AddRedirect(UrlTrackerModel entry);
		bool AddRedirect(IContent newContent, IPublishedContent oldContent, UrlTrackerRedirectType redirectType, UrlTrackerReason reason, string culture = "", bool isChild = false);
		bool AddNotFound(string url, int rootNodeId, string referrer);

		#endregion

		#region Get
		UrlTrackerModel GetEntryById(int id);
		UrlTrackerGetResult GetRedirects(int skip, int amount);
		UrlTrackerGetResult GetNotFounds(int skip, int amount);
		UrlTrackerGetResult GetRedirectsByFilter(int skip, int amount, UrlTrackerSortType sortType = UrlTrackerSortType.CreatedDesc, string searchQuery = "");
		UrlTrackerGetResult GetNotFoundsByFilter(int skip, int amount, UrlTrackerSortType sortType = UrlTrackerSortType.LastOccurrenceDesc, string searchQuery = "");
		List<UrlTrackerModel> GetForcedRedirects();
		List<UrlTrackerDomain> GetDomains();
		string GetUrlByNodeId(int nodeId, string culture = "");
		IPublishedContent GetNodeById(int nodeId);
		bool RedirectExist(int redirectNodeId, string oldUrl, string culture = "");
		IEnumerable<UrlTrackerLanguage> GetLanguages();

		#endregion

		#region Update

		void UpdateEntry(UrlTrackerModel entry);
		void Convert410To301ByNodeId(int nodeId);
		void ClearDomains();
		List<UrlTrackerModel> ReloadForcedRedirectsCache();

		#endregion

		#region Delete

		void DeleteEntryById(int id, bool is404 = false);
		void DeleteEntryByRedirectNodeId(int nodeId);

		#endregion
	}
}