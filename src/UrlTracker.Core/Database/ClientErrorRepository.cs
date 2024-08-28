using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence.Repositories.Implement;
using Umbraco.Cms.Infrastructure.Persistence.SqlSyntax;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Factories;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Models;
using static Umbraco.Cms.Core.Constants;

namespace UrlTracker.Core.Database
{
    [ExcludeFromCodeCoverage]
    public class ClientErrorRepository
        : EntityRepositoryBase<int, IClientError>, IClientErrorRepository
    {
        private const string _referrerTableAlias = "r";
        private const string _mostCommonReferrerTableAlias = "mcr";
        private const string _occurrancesTableAlias = "ref";

        public ClientErrorRepository(IScopeAccessor scopeAccessor,
                                     AppCaches appCaches,
                                     ILogger<EntityRepositoryBase<int, IClientError>> logger)
            : base(scopeAccessor, appCaches, logger)
        { }

        #region Old implementation
        public Task<IReadOnlyCollection<IClientError>> GetAsync(IEnumerable<string> urlsAndPaths)
        {
            IQuery<IClientError> query = SqlContext.Query<IClientError>().Where(e => urlsAndPaths.Contains(e.Url));
            var results = Get(query);
            return Task.FromResult<IReadOnlyCollection<IClientError>>(results.ToList());
        }

        public Task<IReadOnlyCollection<IClientError>> GetNoLongerExistsAsync(IEnumerable<string> urlsAndPaths)
        {
            IQuery<IClientError> query = SqlContext.Query<IClientError>()
                .Where(e => e.Strategy == Defaults.DatabaseSchema.ClientErrorStrategies.NoLongerExists)
                .Where(e => urlsAndPaths.Contains(e.Url));
            var results = Get(query);
            return Task.FromResult<IReadOnlyCollection<IClientError>>(results.ToList());
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

        protected override IClientError? PerformGet(int id)
        {
            var sql = GetBaseQuery(false);
            sql.Where<ClientErrorDto>(e => e.Id == id);
            var dto = Database.Fetch<ClientErrorDto>(sql).FirstOrDefault();

            if (dto is null) return null;

            return ClientErrorFactory.BuildEntity(dto);
        }

        protected override IEnumerable<IClientError> PerformGetAll(params int[]? ids)
        {
            var sql = GetBaseQuery(false);

            if (ids?.Any() is true) sql.WhereIn<ClientErrorDto>(e => e.Id, ids);

            var dtos = Database.Fetch<ClientErrorDto>(sql);
            return dtos.Select(ClientErrorFactory.BuildEntity);
        }

        protected override IEnumerable<IClientError> PerformGetByQuery(IQuery<IClientError> query)
        {
            var sql = GetBaseQuery(false);

            var translator = new SqlTranslator<IClientError>(sql, query);
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

        public void Report(IClientError clientError, DateTime moment, IReferrer? referrer)
        {
            var dto = new ClientError2ReferrerDto
            {
                ClientError = clientError.Id,
                CreateDate = moment,
                Referrer = referrer?.Id
            };

            Database.Insert(dto);
        }

        public async Task<IEnumerable<ReferrerResponse>> GetReferrersByClientIdAsync(int id)
        {
            var sql = Sql().SelectCount("occurrances")
                            .AndSelect<ReferrerDto>("r", r => r.Url)
                            .From<ClientError2ReferrerDto>("cr")
                            .LeftJoin<ReferrerDto>("r")
                            .On<ClientError2ReferrerDto, ReferrerDto>((l, r) => l.Referrer == r.Id, "cr", "r")
                            .Where<ClientError2ReferrerDto>(e => e.ClientError == id, "cr")
                            .GroupBy("cr.referrer", "r.url")
                            .OrderByDescending("occurrances");

            var dtos = await Database.FetchAsync<OccurrancesDto>(sql);
            return dtos.Select(ReferrerFactory.Build);
        }

        public async Task<IEnumerable<DailyClientErrorResponse>> GetDailyClientErrorInRangeAsync(int clientError, DateTime start, DateTime end)
        {
            var query = Sql().Select("CAST(createDate AS date) AS dateOnly")
                        .AndSelectCount("occurances")
                        .From<ClientError2ReferrerDto>()
                        .Where<ClientError2ReferrerDto>(e => e.CreateDate >= start
                                                        && e.CreateDate <= end
                                                        && e.ClientError == clientError)
                        .GroupBy("CAST(createDate AS date)");
            var dtos = await Database.FetchAsync<DailyClientErrorDto>(query);
            return dtos.Select(DailyClientErrorFactory.Build);
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

            var mostCommonReferrers = SqlContext.Templates.Get("mostCommonReferrers", sql
                => sql.Select<ClientError2ReferrerDto>(_referrerTableAlias, e => e.ClientError, e => e.Referrer)
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

        public async Task CleanupAsync(DateTime upperDate)
        {
            // Delete old registrations
            await Database.DeleteManyAsync<ClientError2ReferrerDto>()
                .Where(e => e.CreateDate < upperDate)
                .Execute();

            // Delete unused referrers
            var deleteReferrersQuery = Sql()
                .Delete<ReferrerDto>()
                .WhereNotIn<ReferrerDto>(e => e.Id,
                    Sql()
                    .Select<ClientError2ReferrerDto>(e => e.Referrer)
                    .From<ClientError2ReferrerDto>());

            await Database.ExecuteAsync(deleteReferrersQuery);

            // Delete unreferred client errors
            var deleteClientErrorsQuery = Sql()
                .Delete<ClientErrorDto>()
                .WhereNotIn<ClientErrorDto>(e => e.Id,
                    Sql()
                    .Select<ClientError2ReferrerDto>(e => e.ClientError)
                    .From<ClientError2ReferrerDto>());

            await Database.ExecuteAsync(deleteClientErrorsQuery);
        }
    }
}
