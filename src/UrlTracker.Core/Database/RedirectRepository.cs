using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NPoco;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    [ExcludeFromCodeCoverage]
    public class RedirectRepository
        : IRedirectRepository
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoMapper _mapper;

        public RedirectRepository(IScopeProvider scopeProvider, IUmbracoMapper mapper)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
        }

        public async Task<UrlTrackerRedirect> AddAsync(UrlTrackerRedirect redirect)
        {
            var entry = _mapper.Map<UrlTrackerEntry>(redirect);
            using (var scope = _scopeProvider.CreateScope())
            {
                await scope.Database.InsertAsync(entry);

                var result = _mapper.Map<UrlTrackerRedirect>(entry);

                scope.Complete();
                return result;
            }
        }

        public async Task<UrlTrackerRedirect> UpdateAsync(UrlTrackerRedirect redirect)
        {
            var entry = _mapper.Map<UrlTrackerEntry>(redirect);
            using (var scope = _scopeProvider.CreateScope())
            {
                await scope.Database.UpdateAsync(entry);

                var result = _mapper.Map<UrlTrackerRedirect>(entry);

                scope.Complete();
                return result;
            }
        }

        public async Task<UrlTrackerRedirectCollection> GetAsync()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var query = scope.SqlContext.Sql()
                                            .SelectAll()
                                            .From<UrlTrackerEntry>()
                                            .Where<UrlTrackerEntry>(e => !e.Is404)
                                            .Where<UrlTrackerEntry>(e => e.RedirectHttpCode >= 300 && e.RedirectHttpCode < 400)
                                            .OrderBy<UrlTrackerEntry>(false, e => e.Id);
                var records = await scope.Database.FetchAsync<UrlTrackerEntry>(query);
                var redirects = _mapper.MapEnumerable<UrlTrackerEntry, UrlTrackerRedirect>(records);

                return UrlTrackerRedirectCollection.Create(redirects);
            }
        }

        public async Task<UrlTrackerRedirectCollection> GetAsync(uint skip, uint take, string query, OrderBy order, bool descending)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var countQuery = scope.SqlContext.Sql().SelectCount();
                countQuery = PopulateRedirectQuery(countQuery);

                Task<int> totalRecordsTask = scope.Database.ExecuteScalarAsync<int>(countQuery);

                var selectQuery = scope.SqlContext.Sql().SelectAll();
                selectQuery = PopulateRedirectQuery(selectQuery);
                Expression<Func<UrlTrackerEntry, object>> orderParameter;
                switch (order)
                {
                    case OrderBy.Created: orderParameter = e => e.Inserted; break;
                    case OrderBy.Culture: orderParameter = e => e.Culture; break;
                    case OrderBy.Occurrences:
                    case OrderBy.LastOccurrence:
                    default: throw new ArgumentOutOfRangeException(nameof(order));
                }

                selectQuery = selectQuery.OrderBy<UrlTrackerEntry>(descending, orderParameter);

                List<UrlTrackerEntry> records = await scope.Database.SkipTakeAsync<UrlTrackerEntry>(skip, take, selectQuery);
                var redirects = _mapper.MapEnumerable<UrlTrackerEntry, UrlTrackerRedirect>(records);

                return UrlTrackerRedirectCollection.Create(redirects, await totalRecordsTask);
            }

            Sql<ISqlContext> PopulateRedirectQuery(Sql<ISqlContext> q)
            {
                q = q.From<UrlTrackerEntry>()
                     .Where<UrlTrackerEntry>(e => !e.Is404)
                     .Where<UrlTrackerEntry>(e => e.RedirectHttpCode >= 300 && e.RedirectHttpCode < 400);
                if (!(query is null))
                {
                    bool queryIsInt = int.TryParse(query, out var queryInt);
                    q = q.Where<UrlTrackerEntry>(e => e.OldUrl.Contains(query)
                                                      || e.OldRegex.Contains(query)
                                                      || e.RedirectUrl.Contains(query)
                                                      || e.Notes.Contains(query)
                                                      || (queryIsInt && (e.RedirectNodeId == queryInt)));
                }

                return q;
            }
        }

        /*
         * ToDo: The original code only gets entries from the database where the rootnodeid and the culture intercept those
         *      that are found in the domain provider, so that's what we keep doing here. This doesn't make any sense to me,
         *      because the incoming url doesn't make any claims about culture yet.
         *      It only makes sense once you actually redirect. When you redirect, you'll want to redirect to a particular domain
         *      that may be bound to a culture. By selecting a root node id and a culture, one can select the right domain to
         *      redirect to, while also preserving the freedom to change the culture and domains on each node. That would prevent
         *      the url tracker from redirecting to an old domain.
         *      
         *      The new approach does intercepting in a specific order:
         *       - First attempt to intercept on a set of urls and paths
         *       - At last attempt to intercept by regex (at this point, the domain shouldn't matter anymore)
         *       
         * Bonus: It would be awesome if somebody changes an existing domain, that we insert all required redirects to redirect
         *      the old domain to the new one
         */
        public async Task<IReadOnlyCollection<UrlTrackerShallowRedirect>> GetShallowAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string culture = null)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                // get base query
                var query = scope.SqlContext.Sql()
                    .SelectAll()
                    .From<UrlTrackerEntry>()
                    .Where<UrlTrackerEntry>(entry => urlsAndPaths.Contains(entry.OldUrl))
                    .Where<UrlTrackerEntry>(entry => entry.RedirectHttpCode >= 300 && entry.RedirectHttpCode < 400);

                if (rootNodeId.HasValue)
                {
                    // intercept on root node id if it has been given. Rows without root node id should also be returned
                    query = query.Where<UrlTrackerEntry>(entry => entry.RedirectRootNodeId == rootNodeId || entry.RedirectRootNodeId == null);
                }
                if (!string.IsNullOrWhiteSpace(culture))
                {
                    // intercept on culture if it has been given. Rows without culture should also be returned
                    query = query.Where<UrlTrackerEntry>(entry => entry.Culture == culture || entry.Culture == null);
                }

                query = query.OrderBy<UrlTrackerEntry>(true, e => e.ForceRedirect, e => e.Inserted);

                // return entries as redirects
                var entries = await scope.Database.FetchAsync<UrlTrackerEntry>(query).ConfigureAwait(false);
                return _mapper.MapEnumerable<UrlTrackerEntry, UrlTrackerShallowRedirect>(entries);
            }
        }

        public async Task<IReadOnlyCollection<UrlTrackerShallowRedirect>> GetShallowWithRegexAsync()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                Sql<ISqlContext> sql =
                    scope.SqlContext.Sql()
                                    .SelectAll()
                                    .From<UrlTrackerEntry>()
                                    .Where<UrlTrackerEntry>(e => e.OldRegex != null && e.OldRegex != "")
                                    .OrderBy<UrlTrackerEntry>(true, e => e.ForceRedirect, e => e.Inserted);
                var entries = await scope.Database.FetchAsync<UrlTrackerEntry>(sql).ConfigureAwait(false);
                return _mapper.MapEnumerable<UrlTrackerEntry, UrlTrackerShallowRedirect>(entries);
            }
        }
    }
}
