using System.Collections.Generic;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    public interface IRedirectRepository
    {
        Task<IReadOnlyCollection<UrlTrackerShallowRedirect>> GetShallowAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string culture = null);
        Task<IReadOnlyCollection<UrlTrackerShallowRedirect>> GetShallowWithRegexAsync();
        Task<UrlTrackerRedirectCollection> GetAsync(uint skip, uint take, string query, OrderBy order, bool descending);
        Task<UrlTrackerRedirect> AddAsync(UrlTrackerRedirect redirect);
        Task<UrlTrackerRedirect> UpdateAsync(UrlTrackerRedirect redirect);
        Task<UrlTrackerRedirectCollection> GetAsync();
    }
}