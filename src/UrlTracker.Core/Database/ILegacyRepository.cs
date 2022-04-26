using System;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    [Obsolete("Legacy repository provides support for old functionality. For new features, use IRedirectRepository or IClientErrorRepository")]
    public interface ILegacyRepository
    {
        Task DeleteAsync(UrlTrackerEntry entry);
        Task DeleteAsync(string culture, string sourceUrl, int? targetRootNodeId, bool is404);
        Task<UrlTrackerEntry> GetAsync(int id);
        Task<bool> IsIgnoredAsync(string url);
    }
}