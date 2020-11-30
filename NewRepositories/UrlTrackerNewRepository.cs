using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using Umbraco.Core.Cache;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Web;

namespace InfoCaster.Umbraco.UrlTracker.NewRepositories
{
	public class UrlTrackerNewRepository : IUrlTrackerNewRepository
	{
		private readonly IScopeProvider _scopeProvider;
		private readonly IUrlTrackerCacheService _urlTrackerCacheService;
		private readonly IUrlTrackerNewSettings _urlTrackerSettings;
		private readonly IUrlTrackerNewLoggingHelper _urlTrackerLoggingHelper;

		private readonly string _forcedRedirectsCacheKey = "UrlTrackerForcedRedirects";

		public UrlTrackerNewRepository(
			IScopeProvider scopeProvider,
			IUrlTrackerCacheService urlTrackerCacheService,
			IUrlTrackerNewSettings urlTrackerSettings,
			IUrlTrackerNewLoggingHelper urlTrackerLoggingHelper
			)
		{
			_scopeProvider = scopeProvider;
			_urlTrackerCacheService = urlTrackerCacheService;
			_urlTrackerSettings = urlTrackerSettings;
			_urlTrackerLoggingHelper = urlTrackerLoggingHelper;
		}

		#region Create

		public bool AddEntry(UrlTrackerModel entry)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				string query = "INSERT INTO icUrlTracker (Culture, OldUrl, OldRegex, RedirectRootNodeId, RedirectNodeId, RedirectUrl, RedirectHttpCode, RedirectPassThroughQueryString, ForceRedirect, Notes, Is404, Referrer) VALUES (@culture, @oldUrl, @oldRegex, @redirectRootNodeId, @redirectNodeId, @redirectUrl, @redirectHttpCode, @redirectPassThroughQueryString, @forceRedirect, @notes, @is404, @referrer)";

				var parameters = new
				{
					culture = entry.Culture,
					oldUrl = entry.OldUrl,
					oldRegex = entry.OldRegex,
					redirectRootNodeId = entry.RedirectRootNodeId,
					redirectNodeId = entry.RedirectNodeId,
					redirectUrl = entry.RedirectUrl,
					redirectHttpCode = entry.RedirectHttpCode,
					redirectPassThroughQueryString = entry.RedirectPassThroughQueryString,
					forceRedirect = entry.ForceRedirect,
					notes = entry.Notes,
					is404 = entry.Is404,
					referrer = entry.Referrer
				};

				return scope.Database.Execute(query, parameters) == 1;
			}
		}

		#endregion

		#region Get

		public T FirstOrDefault<T>(string query, object parameters = null)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.FirstOrDefault<T>(query, parameters);
			}
		}

		public UrlTrackerModel GetEntryById(int id)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.SingleOrDefault<UrlTrackerModel>("SELECT * FROM icUrlTracker WHERE Id = @id", new { id = id });
			}
		}

		public UrlTrackerGetResult GetRedirects(int? skip, int? amount, UrlTrackerSortType sort = UrlTrackerSortType.CreatedDesc, string searchQuery = "", bool onlyForcedRedirects = false)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				var result = new UrlTrackerGetResult();
				var searchQueryInt = 0;

				var query = new StringBuilder("SELECT COUNT(*) FROM icUrlTracker WHERE is404 = 0");

				if (!string.IsNullOrEmpty(searchQuery))
				{
					query.Append(" AND (OldUrl LIKE @searchQuery OR OldRegex LIKE @searchQuery OR RedirectUrl LIKE @searchQuery OR Notes LIKE @searchQuery");
					if (int.TryParse(searchQuery, out searchQueryInt))
						query.Append(" OR RedirectNodeId = @searchQueryInt");
					query.Append(")");
				}

				if (onlyForcedRedirects)
					query.Append(" AND ForceRedirect = 1");

				var parameters = new
				{
					skip = skip,
					amount = amount,
					searchQuery = $"%{searchQuery}%",
					searchQueryInt = searchQueryInt
				};

				result.TotalRecords = scope.Database.ExecuteScalar<int>(query.ToString(), parameters);

				if (sort == UrlTrackerSortType.CreatedDesc)
					query.Append(" ORDER BY Inserted DESC");
				else if (sort == UrlTrackerSortType.CreatedAsc)
					query.Append(" ORDER BY Inserted ASC");

				query.Replace("SELECT COUNT(*)", "SELECT *");

				if (skip != null)
					query.Append(" OFFSET @skip ROWS");
				if (amount != null)
					query.Append(" FETCH NEXT @amount ROWS ONLY");

				result.Records = scope.Database.Fetch<UrlTrackerModel>(query.ToString(), parameters);

				return result;
			}
		}

		public UrlTrackerGetResult GetNotFounds(int? skip, int? amount, UrlTrackerSortType sort = UrlTrackerSortType.LastOccurrenceDesc, string searchQuery = "")
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				var result = new UrlTrackerGetResult();
				var query = new StringBuilder("SELECT COUNT(*) FROM (SELECT e.OldUrl FROM icUrlTracker AS e WHERE e.Is404 = 1");

				if (!string.IsNullOrEmpty(searchQuery))
					query.Append(" AND (e.OldUrl LIKE @searchQuery)"); //ToDo: Search on referrer

				query.Append(" GROUP BY e.Culture, e.OldUrl, e.RedirectRootNodeId, e.Is404) result");

				var parameters = new
				{
					skip = skip,
					amount = amount,
					searchQuery = $"%{searchQuery}%",
				};

				result.TotalRecords = scope.Database.ExecuteScalar<int>(query.ToString(), parameters);

				if (sort == UrlTrackerSortType.LastOccurrenceDesc)
					query.Append(" ORDER BY Inserted DESC");
				else if (sort == UrlTrackerSortType.LastOccurrenceAsc)
					query.Append(" ORDER BY Inserted ASC");
				else if (sort == UrlTrackerSortType.OccurrencesDesc)
					query.Append(" ORDER BY Occurrences DESC");
				else if (sort == UrlTrackerSortType.OccurrencesAsc)
					query.Append(" ORDER BY Occurrences ASC");

				var newSelect = new StringBuilder("SELECT * FROM (SELECT MAX(e.Id) AS Id, e.Culture, e.OldUrl, e.RedirectRootNodeId, MAX(e.Inserted) as Inserted, COUNT(e.OldUrl) AS Occurrences, e.Is404");
				newSelect.Append(", Referrer = (SELECT TOP(1) r.Referrer AS Occurrenced FROM icUrlTracker AS r WHERE r.is404 = 1 AND r.OldUrl = e.OldUrl GROUP BY r.Referrer ORDER BY COUNT(r.Referrer) DESC)");

				query.Replace("SELECT COUNT(*) FROM (SELECT e.OldUrl", newSelect.ToString());

				if (skip != null)
					query.Append(" OFFSET @skip ROWS");
				if (amount != null)
					query.Append(" FETCH NEXT @amount ROWS ONLY");

				result.Records = scope.Database.Fetch<UrlTrackerModel>(query.ToString(), parameters);

				return result;
			}
		}

		public bool RedirectExist(int redirectNodeId, string oldUrl, string culture)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.ExecuteScalar<bool>(
					"SELECT 1 FROM icUrlTracker WHERE RedirectNodeId = @redirectNodeId AND OldUrl = @oldUrl AND Culture = @culture",
					new { redirectNodeId = redirectNodeId, oldUrl = oldUrl, culture = culture });
			}
		}

		#endregion

		#region Update

		public void Execute(string query, object parameters = null)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				scope.Database.Execute(query, parameters);
			}
		}

		public void UpdateEntry(UrlTrackerModel entry)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				string query = "UPDATE icUrlTracker SET Culture = @culture, OldUrl = @oldUrl, OldRegex = @oldRegex, RedirectRootNodeId = @redirectRootNodeId, RedirectNodeId = @redirectNodeId, RedirectUrl = @redirectUrl, RedirectHttpCode = @redirectHttpCode, RedirectPassThroughQueryString = @redirectPassThroughQueryString, ForceRedirect = @forceRedirect, Notes = @notes, Is404 = @is404 WHERE Id = @id";

				var parameters = new
				{
					culture = entry.Culture,
					oldUrl = entry.OldUrl,
					oldRegex = entry.OldRegex,
					redirectRootNodeId = entry.RedirectRootNodeId,
					redirectNodeId = entry.RedirectNodeId,
					redirectUrl = entry.RedirectUrl,
					redirectHttpCode = entry.RedirectHttpCode,
					redirectPassThroughQueryString = entry.RedirectPassThroughQueryString,
					forceRedirect = entry.ForceRedirect,
					notes = entry.Notes,
					is404 = entry.Is404,
					id = entry.Id
				};

				scope.Database.Execute(query, parameters);
			}
		}

		public void Convert410To301ByNodeId(int nodeId)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				scope.Database.Execute("UPDATE icUrlTracker SET RedirectHttpCode = 301 WHERE RedirectHttpCode = 410 AND RedirectNodeId = @redirectNodeId", new { redirectNodeId = nodeId });
			}
		}

		#endregion

		#region Delete

		public void DeleteEntryById(int id)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				scope.Database.Execute("DELETE FROM icUrlTracker WHERE Id = @id", new { id = id });
				scope.Complete();
			}
		}

		public bool DeleteEntryByRedirectNodeId(int nodeId)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				var exist = scope.Database.ExecuteScalar<int>(
					"SELECT 1 FROM icUrlTracker WHERE RedirectNodeId = @nodeId AND RedirectHttpCode != 410",
					new { nodeId = nodeId });

				if (exist == 1)
				{
					_urlTrackerLoggingHelper.LogInformation("UrlTracker Repository | Deleting Url Tracker entry of node with id: {0}", nodeId);

					scope.Database.Execute(
						"DELETE FROM icUrlTracker WHERE RedirectNodeId = @nodeId AND RedirectHttpCode != 410",
						new { nodeId = nodeId });

					return true;
				}

				return false;
			}
		}

		public bool DeleteNotFounds(string url, int rootNodeId)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.Execute(
					"DELETE FROM icUrlTracker WHERE OldUrl = @url AND RedirectRootNodeId = @rootNodeId AND Is404 = 1",
					new
					{
						url = url,
						rootNodeId = rootNodeId
					}
				) == 1;
			}
		}

		#endregion

		#region Support

		public bool DoesUrlTrackerTableExists()
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.ExecuteScalar<int>("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'icUrlTracker'") == 1;
			}
		}

		public bool DoesUrlTrackerOldTableExists()
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.ExecuteScalar<int>("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName", new { tableName = _urlTrackerSettings.GetOldTableName() }) == 1;
			}
		}

		#endregion
	}
}