using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NPoco;
using Org.BouncyCastle.Crypto;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence.Repositories.Implement;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Factories;

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

        public async Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, RedirectFilters filters, bool descending)
        {
            var countQuery = Sql().SelectCount();
            countQuery = PopulateRedirectQuery(countQuery);

            Task<int> totalRecordsTask = Database.ExecuteScalarAsync<int>(countQuery);

            var selectQuery = Sql().SelectAll();
            selectQuery = PopulateRedirectQuery(selectQuery);
            selectQuery = selectQuery.OrderBy<RedirectDto>(descending, e => e.CreateDate);

            List<RedirectDto> records = await Database.SkipTakeAsync<RedirectDto>(skip, take, selectQuery);
            var redirects = records.Select(RedirectFactory.BuildEntity);

            return RedirectEntityCollection.Create(redirects, await totalRecordsTask);

            Sql<ISqlContext> PopulateRedirectQuery(Sql<ISqlContext> q)
            {
                q = q.From<RedirectDto>();
                if (query is not null)
                {
                    q = q.Where<RedirectDto>(e => e.SourceValue!.Contains(query)
                                               || e.TargetValue!.Contains(query));
                }

                /* There are three options for redirect types:
                 * - Only permanent
                 * - Only temporary
                 * - Both
                 * 
                 * NOTE: for now it's OK to just check if flag permanent is unequal to flag temporary.
                 *    Should more options become available, this code needs to be updated.
                 */

                if (filters.Types.HasFlag(RedirectType.Temporary) != filters.Types.HasFlag(RedirectType.Permanent))
                {
                    if (filters.Types.HasFlag(RedirectType.Temporary))
                    {
                        q = q.Where<RedirectDto>(e => e.Permanent == false);
                    }

                    if (filters.Types.HasFlag(RedirectType.Permanent))
                    {
                        q = q.Where<RedirectDto>(e => e.Permanent == true);
                    }
                }

                if (filters.SourceTypes?.Any() is true)
                {
                    q = q.WhereIn<RedirectDto>(e => e.SourceStrategy, filters.SourceTypes);
                }

                return q;
            }
        }

        public async Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths)
        {
            // get base query
            var query = Sql()
                .SelectAll()
                .From<RedirectDto>()
                .Where<RedirectDto>(entry => entry.SourceStrategy == Defaults.DatabaseSchema.RedirectSourceStrategies.Url)
                .Where<RedirectDto>(entry => urlsAndPaths.Contains(entry.SourceValue));

            query = query.OrderBy<RedirectDto>(true, e => e.Force, e => e.CreateDate);

            // return entries as redirects
            var entries = await Database.FetchAsync<RedirectDto>(query).ConfigureAwait(false);
            return entries.Select(RedirectFactory.BuildEntity).ToList();
        }

        public Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync()
        {
            var query = AmbientScope.SqlContext.Query<IRedirect>().Where(e => e.SourceStrategy == Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression);
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

        public void DeleteBulk(int[] ids)
        {

            var deleteQuery = Sql().Delete()
                                            .From<RedirectDto>()
                                            .WhereIn<RedirectDto>(e => e.Id, ids);

            Database.Execute(deleteQuery);
        }
    }
}
