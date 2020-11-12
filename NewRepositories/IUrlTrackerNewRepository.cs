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
		UrlTrackerGetResult GetEntries(int? skip, int? amount, UrlTrackerEntryType type, UrlTrackerSortType? sort, string searchQuery = "", bool onlyForcedRedirects = false);
		bool RedirectExist(int redirectNodeId, string oldUrl);
		#endregion

		#region Update

		void Execute(string query, object parameters = null);
		void UpdateEntry(UrlTrackerModel entry);
		void Convert410To301ByNodeId(int nodeId);

		#endregion

		#region Delete

		void DeleteEntryById(int id);
		bool DeleteEntryByRedirectNodeId(int nodeId);

		#endregion

		#region Support

		bool DoesUrlTrackerTableExists();
		bool DoesUrlTrackerOldTableExists();

		#endregion
	}
}
