using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    public interface IRedirectRepository
        : IReadWriteQueryRepository<int, IRedirect>
    {
        Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths);
        Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, OrderBy order, bool descending);
        Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync();
    }
}