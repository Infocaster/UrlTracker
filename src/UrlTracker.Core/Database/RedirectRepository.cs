using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence.Repositories.Implement;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Database.Models.Entities;
using UrlTracker.Core.Database.Models.Factories;

namespace UrlTracker.Core.Database
{
    [ExcludeFromCodeCoverage]
    public class RedirectRepository
        : EntityRepositoryBase<int, IRedirect>, IRedirectRepository
    {
        public RedirectRepository(IScopeAccessor scopeAccessor, AppCaches appCaches, ILogger<EntityRepositoryBase<int, IRedirect>> logger)
            : base(scopeAccessor, appCaches, logger)
        { }

        #region Old Implementation

        public async Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, OrderBy order, bool descending)
        {
            var countQuery = Sql().SelectCount();
            countQuery = PopulateRedirectQuery(countQuery);

            Task<int> totalRecordsTask = Database.ExecuteScalarAsync<int>(countQuery);

            var selectQuery = Sql().SelectAll();
            selectQuery = PopulateRedirectQuery(selectQuery);
            Expression<Func<RedirectDto, object?>> orderParameter = order switch
            {
                OrderBy.Created => e => e.CreateDate,
                OrderBy.Culture => e => e.Culture!,
                _ => throw new ArgumentOutOfRangeException(nameof(order)),
            };
            selectQuery = selectQuery.OrderBy<RedirectDto>(descending, orderParameter);

            List<RedirectDto> records = await Database.SkipTakeAsync<RedirectDto>(skip, take, selectQuery);
            var redirects = records.Select(RedirectFactory.BuildEntity);

            return RedirectEntityCollection.Create(redirects, await totalRecordsTask);

            Sql<ISqlContext> PopulateRedirectQuery(Sql<ISqlContext> q)
            {
                q = q.From<RedirectDto>();
                if (query is not null)
                {
                    bool queryIsInt = int.TryParse(query, out var queryInt);
                    q = q.Where<RedirectDto>(e => e.SourceUrl!.Contains(query)
                                                      || e.SourceRegex!.Contains(query)
                                                      || e.TargetUrl!.Contains(query)
                                                      || e.Notes!.Contains(query)
                                                      || (queryIsInt && (e.TargetNodeId == queryInt)));
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
        public async Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string? culture = null)
        {
            // get base query
            var query = Sql()
                .SelectAll()
                .From<RedirectDto>()
                .Where<RedirectDto>(entry => urlsAndPaths.Contains(entry.SourceUrl));

            if (rootNodeId.HasValue)
            {
                // intercept on root node id if it has been given. Rows without root node id should also be returned
                query = query.Where<RedirectDto>(entry => entry.TargetRootNodeId == rootNodeId || entry.TargetRootNodeId == null);
            }
            if (!string.IsNullOrWhiteSpace(culture))
            {
                // intercept on culture if it has been given. Rows without culture should also be returned
                query = query.Where<RedirectDto>(entry => entry.Culture == culture || entry.Culture == null);
            }

            query = query.OrderBy<RedirectDto>(true, e => e.Force, e => e.CreateDate);

            // return entries as redirects
            var entries = await Database.FetchAsync<RedirectDto>(query).ConfigureAwait(false);
            return entries.Select(RedirectFactory.BuildEntity).ToList();
        }

        public Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync()
        {
            var query = AmbientScope.SqlContext.Query<IRedirect>().Where(e => e.SourceRegex != null);
            var entities = Get(query);

            return Task.FromResult<IReadOnlyCollection<IRedirect>>(entities.ToList());
        }
        #endregion

        protected override IRedirect? PerformGet(int id)
        {
            var sql = GetBaseQuery(false);
            sql.Where(GetBaseWhereClause(), new { id });

            var dto = Database.Fetch<RedirectDto>(sql.SelectTop(1)).FirstOrDefault();
            if (dto is null) return null;

            return RedirectFactory.BuildEntity(dto);
        }

        protected override IEnumerable<IRedirect> PerformGetAll(params int[]? ids)
        {
            var sql = GetBaseQuery(false);

            if (ids?.Any() is true) sql.WhereIn<RedirectDto>(e => e.Id, ids);

            var dtos = Database.Fetch<RedirectDto>(sql);
            return dtos.Select(RedirectFactory.BuildEntity);
        }

        protected override IEnumerable<IRedirect> PerformGetByQuery(IQuery<IRedirect> query)
        {
            var sql = GetBaseQuery(false);

            var translator = new SqlTranslator<IRedirect>(sql, query);
            sql = translator.Translate();

            var dtos = Database.Fetch<RedirectDto>(sql);

            return dtos.Select(RedirectFactory.BuildEntity);
        }

        protected override void PersistNewItem(IRedirect entity)
        {
            entity.AddingEntity();
            if (entity.Key == Guid.Empty) entity.Key = Guid.NewGuid();

            var dto = RedirectFactory.BuildDto(entity);
            var id = Convert.ToInt32(Database.Insert(dto));

            entity.Id = id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IRedirect entity)
        {
            entity.UpdatingEntity();

            var dto = RedirectFactory.BuildDto(entity);
            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            var sql = Sql();

            sql = isCount
                ? sql.SelectCount()
                : sql.Select<RedirectDto>();

            sql.From<RedirectDto>();
            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "id = @id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                $"DELETE FROM {Defaults.DatabaseSchema.Tables.Redirect} WHERE id = @id"
            };
            return list;
        }
    }
}
