using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Database.Models.Entities;

namespace UrlTracker.Core.Database
{
    public interface IRedirectRepository
        : IReadWriteQueryRepository<int, IRedirect>
    {
        Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string culture = null);
        Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string query, OrderBy order, bool descending);
        Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync();
    }
}