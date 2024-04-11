using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    public interface IClientErrorRepository
        : IReadWriteQueryRepository<int, IClientError>
    {
        Task<int> CountAsync(DateTime start, DateTime end);
        Task<ClientErrorEntityCollection> GetAsync(uint skip, uint take, string? query, OrderBy order, bool descending);
        Task<IReadOnlyCollection<IClientError>> GetAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string? culture = null);
        Task<IReadOnlyCollection<IClientError>> GetNoLongerExistsAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string? culture = null);
        void Report(IClientError clientError, DateTime moment, IReferrer? referrer);
        Task<IReadOnlyCollection<IClientErrorMetaData>> GetMetaDataAsync(params int[] clientErrors);
    }
}