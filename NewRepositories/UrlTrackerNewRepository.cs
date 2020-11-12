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
				string query = "INSERT INTO icUrlTracker (OldUrl, OldUrlQueryString, OldRegex, RedirectRootNodeId, RedirectNodeId, RedirectUrl, RedirectHttpCode, RedirectPassThroughQueryString, ForceRedirect, Notes) VALUES (@oldUrl, @oldUrlQueryString, @oldRegex, @redirectRootNodeId, @redirectNodeId, @redirectUrl, @redirectHttpCode, @redirectPassThroughQueryString, @forceRedirect, @notes)";

				var parameters = new
				{
					oldUrl = entry.OldUrl,
					oldUrlQueryString = entry.OldUrlQueryString,
					oldRegex = entry.OldRegex,
					redirectRootNodeId = entry.RedirectRootNodeId,
					redirectNodeId = entry.RedirectNodeId,
					redirectUrl = entry.RedirectUrl,
					redirectHttpCode = entry.RedirectHttpCode,
					redirectPassThroughQueryString = entry.RedirectPassThroughQueryString,
					forceRedirect = entry.ForceRedirect,
					notes = entry.Notes
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

			return default(T);
		}

		public UrlTrackerModel GetEntryById(int id)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.SingleOrDefault<UrlTrackerModel>("SELECT * FROM icUrlTracker WHERE Id = @id", new {id = id});
			}
		}

		public UrlTrackerGetResult GetEntries(int? skip, int? amount, UrlTrackerEntryType type, UrlTrackerSortType? sort, string searchQuery = "", bool onlyForcedRedirects = false)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				var result = new UrlTrackerGetResult();
				var searchQueryInt = 0;

				var query = new StringBuilder("SELECT COUNT(*) FROM icUrlTracker WHERE");

				if (type == UrlTrackerEntryType.Redirect)
					query.Append(" is404 = 0");
				else if (type == UrlTrackerEntryType.NotFound)
					query.Append(" is404 = 1");

				if (!string.IsNullOrEmpty(searchQuery))
				{
					query.Append(" AND (OldUrl LIKE @searchQuery OR OldUrlQueryString LIKE @searchQuery OR OldRegex LIKE @searchQuery OR RedirectUrl LIKE @searchQuery OR Notes LIKE @searchQuery");
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

				if (sort != null || skip != null || amount != null)
				{
					if (sort == UrlTrackerSortType.CreatedDesc || sort == null)
						query.Append(" ORDER BY Inserted DESC");
					else if (sort == UrlTrackerSortType.CreatedAsc)
						query.Append(" ORDER BY Inserted ASC");
					else if (sort == UrlTrackerSortType.OccurencedDesc)
						query.Append(" ORDER BY Inserted DESC");
					else if (sort == UrlTrackerSortType.OccurencedAsc)
						query.Append(" ORDER BY Inserted ASC");
					else if (sort == UrlTrackerSortType.LastOccurenceDesc)
						query.Append(" ORDER BY Inserted DESC");
					else if (sort == UrlTrackerSortType.LastOccurenceAsc)
						query.Append(" ORDER BY Inserted ASC");
				}

				query.Replace("SELECT COUNT(*)", "SELECT *");

				if (skip != null)
					query.Append(" OFFSET @skip ROWS");
				if (amount != null)
					query.Append(" FETCH NEXT @amount ROWS ONLY");

				result.Records = scope.Database.Fetch<UrlTrackerModel>(query.ToString(), parameters);

				return result;
			}

			return new UrlTrackerGetResult();

		}

		public bool RedirectExist(int redirectNodeId, string oldUrl)
		{
			using (var scope = _scopeProvider.CreateScope(autoComplete: true))
			{
				return scope.Database.ExecuteScalar<bool>("SELECT 1 FROM icUrlTracker WHERE RedirectNodeId = @redirectNodeId AND OldUrl = @oldUrl", new { redirectNodeId = redirectNodeId, oldUrl = oldUrl });
			}
		}

		#endregion Get

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
				string query = "UPDATE icUrlTracker SET OldUrl = @oldUrl, OldUrlQueryString = @oldUrlQueryString, OldRegex = @oldRegex, RedirectRootNodeId = @redirectRootNodeId, RedirectNodeId = @redirectNodeId, RedirectUrl = @redirectUrl, RedirectHttpCode = @redirectHttpCode, RedirectPassThroughQueryString = @redirectPassThroughQueryString, ForceRedirect = @forceRedirect, Notes = @notes, Is404 = @is404 WHERE Id = @id";

				var parameters = new
				{
					oldUrl = entry.OldUrl,
					oldUrlQueryString = entry.OldUrlQueryString,
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
				scope.Database.Execute("DELETE FROM icUrlTracker WHERE Id = @id", new {id = id});
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