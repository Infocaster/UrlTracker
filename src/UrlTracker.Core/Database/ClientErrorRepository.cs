using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    [ExcludeFromCodeCoverage]
    public class ClientErrorRepository
        : IClientErrorRepository
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoMapper _mapper;

        public ClientErrorRepository(IScopeProvider scopeProvider,
                                     IUmbracoMapper mapper)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<UrlTrackerShallowClientError>> GetShallowAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string? culture = null)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var query = scope.SqlContext.Sql().SelectAll()
                .From<UrlTrackerEntry>()
                .Where<UrlTrackerEntry>(e => urlsAndPaths.Contains(e.OldUrl))
                .Where<UrlTrackerEntry>(e => e.Is404 || (e.RedirectHttpCode >= 400 && e.RedirectHttpCode < 500));

            if (rootNodeId.HasValue)
            {
                query = query.Where<UrlTrackerEntry>(e => e.RedirectRootNodeId == rootNodeId || e.RedirectRootNodeId == null);
            }

            if (!string.IsNullOrWhiteSpace(culture))
            {
                query = query.Where<UrlTrackerEntry>(e => e.Culture == culture || e.Culture == null);
            }

            var entries = await scope.Database.FetchAsync<UrlTrackerEntry>(query).ConfigureAwait(false);
            return _mapper.MapEnumerable<UrlTrackerEntry, UrlTrackerShallowClientError>(entries);
        }

        public async Task<int> CountAsync(DateTime start, DateTime end)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var query = scope.SqlContext.Sql().SelectCount()
                .From<UrlTrackerEntry>()
                .Where<UrlTrackerEntry>(e => e.Is404)
                .Where<UrlTrackerEntry>(e => e.Inserted >= start && e.Inserted <= end);

            return await scope.Database.ExecuteScalarAsync<int>(query).ConfigureAwait(false);
        }

        public async Task<UrlTrackerNotFound> AddAsync(UrlTrackerNotFound notFound)
        {
            var entry = _mapper.Map<UrlTrackerEntry>(notFound)!;
            using var scope = _scopeProvider.CreateScope();
            await scope.Database.InsertAsync(entry).ConfigureAwait(false);
            var result = _mapper.Map<UrlTrackerNotFound>(entry)!;

            scope.Complete();
            return result;
        }

        // ToDo: long and complicated method, can we do this more efficient?
        public async Task<UrlTrackerRichNotFoundCollection> GetAsync(uint skip, uint take, string? query, OrderBy order, bool descending)
        {
            const string tableAlias = "e";
            const string subQueryAlias = "re";
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var countQuery = scope.SqlContext.Sql()
                .SelectCount().From("e", sql => // Table alias is not important here, because we're only interested in the row count
                {
                    sql.SelectMax<UrlTrackerEntry>("Id", tableAlias, e => e.Id)
                                             .From<UrlTrackerEntry>(tableAlias)
                                             .Where<UrlTrackerEntry>(e => e.Is404, tableAlias);
                    if (query is not null)
                    {
                        sql = sql.Where<UrlTrackerEntry>(e => e.OldUrl!.Contains(query), tableAlias);
                    }

                    sql = sql.GroupBy<UrlTrackerEntry>(tableAlias, e => e.OldUrl!, e => e.Culture!, e => e.RedirectRootNodeId!, e => e.Is404);
                });

            Task<int> totalRecordsTask = scope.Database.ExecuteScalarAsync<int>(countQuery);
            var selectQuery = scope.SqlContext.Sql()
                                              .SelectMax<UrlTrackerEntry>("Id", tableAlias, e => e.Id)
                                              .AndSelectMax<UrlTrackerEntry>("Inserted", tableAlias, e => e.Inserted)
                                              .AndSelect<UrlTrackerEntry>(tableAlias,
                                                                          e => e.Is404,
                                                                          e => e.OldUrl,
                                                                          e => e.RedirectRootNodeId,
                                                                          e => e.Culture)
                                              .AndSelectCount("Occurrences")
                                              .AndSelect<UrlTrackerEntry>(subQueryAlias, e => e.Referrer)
                                              .From<UrlTrackerEntry>(tableAlias)
                                              // ToDo: Can this be done strongly typed? I don't know.
                                              .LeftJoin("(SELECT r.[OldUrl], r.[Referrer] FROM (SELECT [OldUrl], [Referrer], ROW_NUMBER() OVER (PARTITION BY [OldUrl] ORDER BY COUNT([Referrer]) DESC) rn FROM [dbo].[icUrlTracker] WHERE [Referrer] IS NOT NULL GROUP BY [OldUrl], [Referrer]) as r WHERE r.rn = 1) re")
                                              .On("[e].[OldUrl] = [re].[OldUrl]")
                                              .Where<UrlTrackerEntry>(e => e.Is404, tableAlias);
            if (query is not null)
            {
                selectQuery = selectQuery.Where<UrlTrackerEntry>(e => e.OldUrl!.Contains(query), tableAlias);
            }
            selectQuery = selectQuery.GroupBy("[e].[OldUrl]", "[e].[Culture]", "[e].[RedirectRootNodeId]", "[e].[Is404]", "[re].[Referrer]");
            string orderParameter = order switch
            {
                OrderBy.LastOccurrence or
                OrderBy.Created => $"MAX({scope.SqlContext.SqlSyntax.GetFieldName<UrlTrackerEntry>(e => e.Inserted, tableAlias)})",
                OrderBy.Culture => scope.SqlContext.SqlSyntax.GetFieldName<UrlTrackerEntry>(e => e.Culture, tableAlias),
                OrderBy.Occurrences => "COUNT(*)",
                _ => throw new ArgumentOutOfRangeException(nameof(order)),
            };
            selectQuery = selectQuery.GenericOrderBy(descending, orderParameter);
            var records = await scope.Database.SkipTakeAsync<UrlTrackerEntryNotFoundAggregate>(skip, take, selectQuery).ConfigureAwait(false);
            var notFounds = _mapper.MapEnumerable<UrlTrackerEntryNotFoundAggregate, UrlTrackerRichNotFound>(records);

            return UrlTrackerRichNotFoundCollection.Create(notFounds, await totalRecordsTask);
        }

        public async Task DeleteAsync(string url, string? culture)
        {
            using var scope = _scopeProvider.CreateScope();
            var query = scope.SqlContext.Sql()
                                        .Delete()
                                        .From<UrlTrackerEntry>()
                                        .Where<UrlTrackerEntry>(e => e.OldUrl == url)
                                        .Where<UrlTrackerEntry>(e => e.Is404);

            if (culture is null) query = query.WhereNull<UrlTrackerEntry>(e => e.Culture);
            else query = query.Where<UrlTrackerEntry>(e => e.Culture == culture);

            await scope.Database.ExecuteAsync(query).ConfigureAwait(false);

            scope.Complete();
        }

        public async Task<UrlTrackerNotFound?> GetAsync(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var query = scope.SqlContext.Sql()
                                        .SelectAll()
                                        .From<UrlTrackerEntry>()
                                        .Where<UrlTrackerEntry>(e => e.Id == id);
            var entries = await scope.Database.FetchAsync<UrlTrackerEntry>(query).ConfigureAwait(false);

            return _mapper.Map<UrlTrackerNotFound>(entries.FirstOrDefault());
        }

        public async Task UpdateAsync(UrlTrackerNotFound entry)
        {
            using var scope = _scopeProvider.CreateScope();
            // This method is only used to ignore 404 entries, so it can pretty much be assumed that ignored is true.
            //    I chose this setup to better accommodate a future rework of the database tables.
            //    It's impossible to set ignore to false, because the not found entries will have been deleted.
            if (entry.Ignored)
            {
                await Task.WhenAll(scope.Database.InsertAsync<UrlTrackerIgnoreEntry>(new UrlTrackerIgnoreEntry
                {
                    Culture = null,
                    RootNodeId = null,
                    Url = entry.Url
                }), DeleteAsync(entry.Url, null));
            }

            scope.Complete();
        }
    }
}
