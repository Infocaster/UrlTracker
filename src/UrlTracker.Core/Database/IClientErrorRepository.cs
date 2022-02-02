using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    public interface IClientErrorRepository
    {
        Task<UrlTrackerNotFound> AddAsync(UrlTrackerNotFound notFound);
        Task<int> CountAsync(DateTime start, DateTime end);
        Task<UrlTrackerRichNotFoundCollection> GetAsync(uint skip, uint take, string query, OrderBy order, bool descending);
        Task<UrlTrackerNotFound> GetAsync(int id);
        Task<IReadOnlyCollection<UrlTrackerShallowClientError>> GetShallowAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string culture = null);
        Task DeleteAsync(string url, string culture);

        // We can't return the updated 404 here, because secretly it's actually deleted
        Task UpdateAsync(UrlTrackerNotFound entry);
    }
}