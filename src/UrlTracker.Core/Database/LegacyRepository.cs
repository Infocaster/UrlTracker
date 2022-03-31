using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Extensions;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    [Obsolete("Legacy repository provides support for old functionality. For new features, use RedirectRepository or ClientErrorRepository")]
    [ExcludeFromCodeCoverage]
    public class LegacyRepository
        : ILegacyRepository
    {
        private readonly IScopeProvider _scopeProvider;

        public LegacyRepository(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task<UrlTrackerEntry?> GetAsync(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var query = scope.SqlContext.Sql()
                                        .SelectAll()
                                        .From<UrlTrackerEntry>()
                                        .Where<UrlTrackerEntry>(e => e.Id == id);
            var results = await scope.Database.FetchAsync<UrlTrackerEntry>(query).ConfigureAwait(false);
            return results.FirstOrDefault();
        }

        public async Task DeleteAsync(string? culture, string sourceUrl, int? targetRootNodeId, bool is404)
        {
            using var scope = _scopeProvider.CreateScope();
            var query = scope.SqlContext.Sql().Delete()
                .From<UrlTrackerEntry>()
                .Where<UrlTrackerEntry>(e => e.OldUrl == sourceUrl)
                .Where<UrlTrackerEntry>(e => e.RedirectRootNodeId == targetRootNodeId)
                .Where<UrlTrackerEntry>(e => e.Is404 == is404);

            if (culture is null) query = query.WhereNull<UrlTrackerEntry>(e => e.Culture);
            else query = query.Where<UrlTrackerEntry>(e => e.Culture == culture);

            await scope.Database.ExecuteAsync(query).ConfigureAwait(false);
            scope.Complete();
        }

        public async Task DeleteAsync(UrlTrackerEntry entry)
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.Database.DeleteAsync(entry).ConfigureAwait(false);
            scope.Complete();
        }
    }
}
