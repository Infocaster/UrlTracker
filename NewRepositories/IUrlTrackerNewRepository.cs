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

		#endregion

		#region Get

		T FirstOrDefault<T>(string query, object parameters = null);
		UrlTrackerModel GetEntryById(int id);
		UrlTrackerGetResult GetRedirects(int? skip, int? amount, UrlTrackerSortType sort = UrlTrackerSortType.CreatedDesc, string searchQuery = "", bool onlyForcedRedirects = false);
		UrlTrackerGetResult GetNotFounds(int? skip, int? amount, UrlTrackerSortType sort = UrlTrackerSortType.LastOccurrenceDesc, string searchQuery = "");
		bool RedirectExist(int redirectNodeId, string oldUrl, string culture = "");

		#endregion

		#region Update

		void Execute(string query, object parameters = null);
		void UpdateEntry(UrlTrackerModel entry);
		void Convert410To301ByNodeId(int nodeId);

		#endregion

		#region Delete

		void DeleteEntryById(int id);
		bool DeleteEntryByRedirectNodeId(int nodeId);
		bool DeleteNotFounds(string url, int rootNodeId);

		#endregion

		#region Support

		bool DoesUrlTrackerTableExists();
		bool DoesUrlTrackerOldTableExists();

		#endregion
	}
}
