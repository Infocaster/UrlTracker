using System.Collections.Generic;
using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database
{
    public interface IReferrerRepository
        : IReadWriteQueryRepository<int, IReferrer>
    {
        IReferrer? Get(string url);
        IEnumerable<IReferrer?> GetAll(string url);
    }
}