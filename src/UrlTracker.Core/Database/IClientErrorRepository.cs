using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Database
{
    public interface IClientErrorRepository
        : IReadWriteQueryRepository<int, IClientError>
    {
        Task<IReadOnlyCollection<IClientError>> GetAsync(IEnumerable<string> urlsAndPaths);
        Task<IReadOnlyCollection<IClientError>> GetNoLongerExistsAsync(IEnumerable<string> urlsAndPaths);
        void Report(IClientError clientError, DateTime moment, IReferrer? referrer);
        Task<IEnumerable<ReferrerResponse>> GetReferrersByClientIdAsync(int id);
        Task<IEnumerable<DailyClientErrorResponse>> GetDailyClientErrorInRangeAsync(int clientError, DateTime start, DateTime end);
        Task<IReadOnlyCollection<IClientErrorMetaData>> GetMetaDataAsync(params int[] clientErrors);
        Task CleanupAsync(DateTime upperDate);
    }
}