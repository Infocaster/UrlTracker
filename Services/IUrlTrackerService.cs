using InfoCaster.Umbraco.UrlTracker.Models;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public interface IUrlTrackerService
	{
		#region Create

		bool AddRedirect(UrlTrackerModel entry);
		bool AddRedirect(IContent newContent, IPublishedContent oldContent, UrlTrackerHttpCode redirectType, UrlTrackerReason reason, string culture = null, bool isChild = false);
		bool AddNotFound(string url, int rootNodeId, string referrer, string culture = null);
		bool AddIgnore404(int id);

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
		IEnumerable<UrlTrackerLanguage> GetLanguagesOutNodeDomains(int nodeId);
		int CountNotFoundsThisWeek();
		UrlTrackerDomain GetUmbracoDomainFromUrl(string url, ref string urlWithoutDomain);
		bool IgnoreExist(string url, int RootNodeId, string culture);

		#endregion

		#region Update

		void UpdateEntry(UrlTrackerModel entry);
		void Convert410To301ByNodeId(int nodeId);
		void ConvertRedirectTo410ByNodeId(int nodeId);
		void ClearDomains();
		void ClearForcedRedirectsCache();

		#endregion

		#region Delete

		bool DeleteEntryById(int id, bool is404 = false);
		void DeleteEntryByRedirectNodeId(int nodeId);

		#endregion
	}
}