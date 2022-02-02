using System;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core
{
    [Obsolete("Legacy service provides support for an old functionality. For new functionality, use RedirectService or ClientErrorService.")]
    public interface ILegacyService
    {
        Task DeleteAsync(UrlTrackerEntry entry);
        Task<UrlTrackerEntry> GetAsync(int id);
    }
}