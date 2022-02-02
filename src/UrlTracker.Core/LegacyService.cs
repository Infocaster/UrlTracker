using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core
{
    [Obsolete("Legacy service provides support for an old functionality. For new functionality, use RedirectService or ClientErrorService.")]
    [ExcludeFromCodeCoverage]
    public class LegacyService
        : ILegacyService
    {
        private readonly ILegacyRepository _legacyRepository;

        public LegacyService(ILegacyRepository legacyRepository)
        {
            _legacyRepository = legacyRepository;
        }

        public Task DeleteAsync(UrlTrackerEntry entry)
        {
            if (entry.Is404)
            {
                return _legacyRepository.DeleteAsync(entry.Culture, entry.OldUrl, entry.RedirectRootNodeId, entry.Is404);
            }
            return _legacyRepository.DeleteAsync(entry);
        }

        public Task<UrlTrackerEntry> GetAsync(int id)
        {
            return _legacyRepository.GetAsync(id);
        }
    }
}
