using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database
{
    public interface IRedirectRepository
        : IReadWriteQueryRepository<int, IRedirect>
    {
        Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths);
        Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, bool descending);
        Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync();
    }
}