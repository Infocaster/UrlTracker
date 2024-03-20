using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NPoco;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Scoping;
using UrlTracker.Core.Compatibility;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Factories;
using UrlTracker.Core.Database.Models;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Core.Database
{
    [ExcludeFromCodeCoverage]
    public class ClientErrorRepository
        : CompatibilityNPocoRepositoryBase<int, IClientError>, IClientErrorRepository
    {
        private const string _referrerTableAlias = "r";
        private const string _mostCommonReferrerTableAlias = "mcr";
        private const string _occurrancesTableAlias = "ref";

        // throws NotImplementedException on purpose, because we don't need to implement it.
        //    (Some repositories in the Umbraco source do this as well)
        protected override Guid NodeObjectTypeId => throw new NotImplementedException();

        public ClientErrorRepository(IScopeAccessor scopeAccessor,
                                     AppCaches appCaches,
                                     ILogger logger)
            : base(scopeAccessor, appCaches, logger)
        { }

        #region Old implementation
        public Task<IReadOnlyCollection<IClientError>> GetAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string culture = null)
        {
            IQuery<IClientError> query = SqlContext.Query<IClientError>().Where(e => urlsAndPaths.Contains(e.Url));
            var results = Get(query);
            return Task.FromResult<IReadOnlyCollection<IClientError>>(results.ToList());
        }

        public async Task<int> CountAsync(DateTime start, DateTime end)
        {
            var query = Sql().SelectCount()
                             .From<ClientError2ReferrerDto>()
                             .Where<ClientError2ReferrerDto>(e => e.CreateDate >= start && e.CreateDate <= end);

            return await Database.ExecuteScalarAsync<int>(query).ConfigureAwait(false);
        }

        public async Task<ClientErrorEntityCollection> GetAsync(uint skip, uint take, string query, OrderBy order, bool descending)
        {
            var countQuery = Sql()
                .SelectCount()
                .From<ClientErrorDto>()
                .Where<ClientErrorDto>(e => e.Ignored == false);

            if (query != null)
            {
                countQuery.Where<ClientErrorDto>(e => e.Url.Contains(query));
            }

            Task<int> totalRecordsTask = Database.ExecuteScalarAsync<int>(countQuery);

            var aggregateQuery = Sql()
                .Select<ClientError2ReferrerDto>(e => e.ClientError)
                .AndSelectCount(Defaults.DatabaseSchema.AggregateColumns.TotalOccurrences)
                .AndSelectMax<ClientError2ReferrerDto>(Defaults.DatabaseSchema.AggregateColumns.MostRecentOccurrence, null, e => e.CreateDate)
                .From<ClientError2ReferrerDto>()
                .GroupBy<ClientError2ReferrerDto>(e => e.ClientError)
                ;

            var selectQuery = Sql()
                .Select<ClientErrorDto>("c")
                .From<ClientErrorDto>("c")
                .LeftJoin(aggregateQuery, "cr").On<ClientError2ReferrerDto, ClientErrorDto>((l, r) => l.ClientError == r.Id, "cr", "c")
                .Where<ClientErrorDto>(e => e.Ignored == false, "c");
            if (query != null)
            {
                selectQuery.Where<ClientErrorDto>(e => e.Url.Contains(query), "c");
            }

            string orderParameter;
            switch (order)
            {
                case OrderBy.LastOccurrence:
                case OrderBy.Created: orderParameter = SqlSyntax.GetQuotedColumnName(Defaults.DatabaseSchema.AggregateColumns.MostRecentOccurrence); break;
                case OrderBy.Occurrences: orderParameter = SqlSyntax.GetQuotedColumnName(Defaults.DatabaseSchema.AggregateColumns.TotalOccurrences); break;
                default: throw new ArgumentOutOfRangeException(nameof(order));
            }

            selectQuery = selectQuery.GenericOrderBy(descending, orderParameter);
            var dtos = await Database.SkipTakeAsync<ClientErrorDto>(skip, take, selectQuery).ConfigureAwait(false);

            return ClientErrorEntityCollection.Create(dtos.Select(ClientErrorFactory.BuildEntity), await totalRecordsTask);
        }
        #endregion

        public override void Delete(IClientError entity)
        {
            base.Delete(entity);

            var deleteReferrersQuery = Sql().Delete()
                                            .From<ReferrerDto>()
                                            .WhereNotIn<ReferrerDto>(e => e.Id, Sql().Select<ClientError2ReferrerDto>(e => e.Referrer)
                                                                                     .From<ClientError2ReferrerDto>());

            Database.Execute(deleteReferrersQuery);
        }

        protected override IClientError PerformGet(int id)
        {
            var sql = GetBaseQuery(false);
            sql.Where<ClientErrorDto>(e => e.Id == id);
            var dto = Database.Fetch<ClientErrorDto>(sql).FirstOrDefault();

            if (dto is null) return null;

            return ClientErrorFactory.BuildEntity(dto);
        }

        protected override IEnumerable<IClientError> PerformGetAll(params int[] ids)
        {
            var sql = GetBaseQuery(false);

            if (ids?.Any() is true) sql.WhereIn<ClientErrorDto>(e => e.Id, ids);

            var dtos = Database.Fetch<ClientErrorDto>(sql);
            return dtos.Select(ClientErrorFactory.BuildEntity);
        }

        protected override IEnumerable<IClientError> PerformGetByQuery(IQuery<IClientError> query)
        {
            var sql = GetBaseQuery(false);

            var translator = new CompatibilitySqlTranslator<IClientError>(sql, query);
            sql = translator.Translate();

            var dtos = Database.Fetch<ClientErrorDto>(sql);
            return dtos.Select(ClientErrorFactory.BuildEntity);
        }

        protected override void PersistNewItem(IClientError entity)
        {
            entity.AddingEntity();
            if (entity.Key == Guid.Empty) entity.Key = Guid.NewGuid();

            var dto = ClientErrorFactory.BuildDto(entity);
            var id = Convert.ToInt32(Database.Insert(dto));

            entity.Id = id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IClientError entity)
        {
            entity.UpdatingEntity();

            var dto = ClientErrorFactory.BuildDto(entity);
            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            var sql = Sql();
            sql = isCount
                ? sql.SelectCount()
                : sql.SelectAll();

            sql = sql.From<ClientErrorDto>();
            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return $"{SqlSyntax.GetFieldName<ClientErrorDto>(e => e.Id)} = @id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                $"DELETE FROM {Defaults.DatabaseSchema.Tables.ClientError2Referrer} WHERE clientError = @id",
                $"DELETE FROM {Defaults.DatabaseSchema.Tables.ClientError} WHERE id = @id"
            };
            return list;
        }

        public void Report(IClientError clientError, DateTime moment, IReferrer referrer)
        {
            var dto = new ClientError2ReferrerDto
            {
                ClientError = clientError.Id,
                CreateDate = moment,
                Referrer = referrer?.Id
            };

            Database.Insert(dto);
        }

        public async Task<IReadOnlyCollection<IClientErrorMetaData>> GetMetaDataAsync(params int[] clientErrors)
        {
            var sql = Sql();

            ISqlSyntaxProvider syntax = sql.SqlContext.SqlSyntax;
            sql = sql.SelectCount(Defaults.DatabaseSchema.AggregateColumns.TotalOccurrences)
                     .AndSelectMax<ClientError2ReferrerDto>(Defaults.DatabaseSchema.AggregateColumns.MostRecentOccurrence, _occurrancesTableAlias, e => e.CreateDate)
                     .AndSelect($"{syntax.GetQuotedColumnName(_occurrancesTableAlias)}.{syntax.GetQuotedColumnName("clientError")} as {syntax.GetQuotedColumnName("clientError")}")
                     .AndSelect($"{syntax.GetQuotedColumnName(_referrerTableAlias)}.{syntax.GetQuotedColumnName("url")} as {syntax.GetQuotedColumnName(Defaults.DatabaseSchema.AggregateColumns.MostCommonReferrer)}");

            sql.From<ClientError2ReferrerDto>(_occurrancesTableAlias);

            var mostCommonReferrers = SqlContext.Templates.Get("mostCommonReferrers", innerSql
                => innerSql.Select<ClientError2ReferrerDto>(_referrerTableAlias, e => e.ClientError, e => e.Referrer)
                      .From(_referrerTableAlias, s => s.Select<ClientError2ReferrerDto>(e => e.ClientError, e => e.Referrer)
                                    .Append(", ROW_NUMBER() over (PARTITION BY [clientError] ORDER BY COUNT([Referrer]) DESC) as rn")
                                    .From<ClientError2ReferrerDto>()
                                    .WhereNotNull<ClientError2ReferrerDto>(e => e.Referrer)
                                    .GroupBy<ClientError2ReferrerDto>(e => e.ClientError, e => e.Referrer))
                      .Where("rn = 1")
                      );

            sql.LeftJoin(mostCommonReferrers.Sql(), _mostCommonReferrerTableAlias)
               .On<ClientError2ReferrerDto, ClientError2ReferrerDto>((l, r) => l.ClientError == r.ClientError, _occurrancesTableAlias, _mostCommonReferrerTableAlias)
               .LeftJoin<ReferrerDto>(_referrerTableAlias)
               .On<ClientError2ReferrerDto, ReferrerDto>((l, r) => l.Referrer == r.Id, _mostCommonReferrerTableAlias, _referrerTableAlias);

            sql.Where<ClientError2ReferrerDto>(e => clientErrors.Contains(e.ClientError), _occurrancesTableAlias)
               .GroupBy<ClientError2ReferrerDto>(_occurrancesTableAlias, e => e.ClientError)
               .Append($", {SqlSyntax.GetQuotedTableName(_referrerTableAlias)}.{SqlSyntax.GetQuotedTableName("url")}");

            var dtos = await Database.FetchAsync<ClientErrorMetaDataDto>(sql);

            return dtos.Select(ClientErrorFactory.BuildEntity).ToList();
        }
    }
}
