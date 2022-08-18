using System;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core
{
    public interface IClientErrorService
    {
        Task<ClientError> AddAsync(ClientError ClientError);
        Task<int> CountAsync(DateTime? start, DateTime? end);
        Task DeleteAsync(ClientError ClientError);
        Task<Models.ClientErrorCollection> GetAsync(uint skip, uint take, string query, OrderBy orderBy, bool descending);
        Task<ClientError> GetAsync(int id);
        Task<ClientError> GetAsync(string url);
        Task UpdateAsync(ClientError ClientError);
        Task ReportAsync(string url, DateTime moment, string referrer);
    }
}