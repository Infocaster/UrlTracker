using System;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core
{
    public interface IClientErrorService
    {
        Task<NotFound> AddAsync(NotFound notFound);
        Task<int> CountAsync(DateTime? start, DateTime? end);
        Task DeleteAsync(string url, string culture);
        Task<RichNotFoundCollection> GetAsync(uint skip, uint take, string query, OrderBy orderBy, bool descending);
        Task<NotFound> GetAsync(int id);
        Task UpdateAsync(NotFound notFound);
    }
}