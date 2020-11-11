using System.Collections.Generic;
using InfoCaster.Umbraco.UrlTracker.Models;
using Lucene.Net.Search;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.NewRepositories
{
	public interface IUrlTrackerNewRepository
	{
		#region Add

		bool AddEntry(UrlTrackerModel entry);
		//bool AddUrlMapping(IContent newContent, UrlTrackerReason reason, bool isChild = false);

		#endregion

		#region Get
		T FirstOrDefault<T>(string query, object parameters = null);
		UrlTrackerGetResult GetRedirects(int? skip, int? amount);
		UrlTrackerGetResult GetNotFounds(int? skip, int? amount);
		UrlTrackerGetResult GetRedirectsByFilter(int? skip, int? amount, UrlTrackerSortType sortType = UrlTrackerSortType.CreatedDesc, string searchQuery = "");
		List<UrlTrackerModel> GetForcedRedirects();
		bool RedirectExist(int redirectNodeId, string oldUrl);
		#endregion

		#region Update

		void Execute(string query, object parameters = null);
		void UpdateEntry(UrlTrackerModel entry);
		void Convert410To301ByNodeId(int nodeId);

		#endregion

		#region Delete

		void DeleteEntryByRedirectNodeId(int nodeId);

		#endregion

		#region Support

		bool DoesUrlTrackerTableExists();
		bool DoesUrlTrackerOldTableExists();

		#endregion

		#region Reload cache

		List<UrlTrackerModel> ReloadForcedRedirectsCache();

		#endregion 
	}
}
