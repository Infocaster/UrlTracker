using System;
using System.Collections.Generic;
using System.Linq;
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
using UrlTracker.Core.Database.Factories;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    internal class RecommendationRepository
        : EntityRepositoryBase<int, IRecommendation>, IRecommendationRepository
    {
        private readonly IRedactionScoreRepository _redactionScoreRepository;

        public RecommendationRepository(IScopeAccessor scopeAccessor, AppCaches appCaches, ILogger<RecommendationRepository> logger, IRedactionScoreRepository redactionScoreRepository)
            : base(scopeAccessor, appCaches, logger)
        {
            _redactionScoreRepository = redactionScoreRepository;
        }

        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            var sql = Sql();
            sql = isCount
                ? sql.SelectCount()
                : sql.Select<RecommendationDto>();

            sql = sql.From<RecommendationDto>();
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
                $"DELETE FROM {Defaults.DatabaseSchema.Tables.Recommendation} WHERE id = @id"
            };
            return list;
        }

        protected override IRecommendation? PerformGet(int id)
        {
            var sql = GetBaseQuery(false);
            sql.Where(GetBaseWhereClause(), new { id });

            var dto = Database.Fetch<RecommendationDto>(sql.SelectTop(1)).FirstOrDefault();
            if (dto is null) return null;

            var strategy = _redactionScoreRepository.Get(dto.RecommendationStrategy);
            return RecommendationFactory.BuildEntity(dto, strategy!);
        }

        protected override IEnumerable<IRecommendation> PerformGetAll(params int[]? ids)
        {
            var sql = GetBaseQuery(false);

            if (ids?.Any() is true) sql.WhereIn<RecommendationDto>(e => e.Id, ids);

            var dtos = Database.Fetch<RecommendationDto>(sql);
            return dtos.Select(dto => RecommendationFactory.BuildEntity(dto, _redactionScoreRepository.Get(dto.RecommendationStrategy)!)).ToList();
        }

        protected override IEnumerable<IRecommendation> PerformGetByQuery(IQuery<IRecommendation> query)
        {
            var sql = GetBaseQuery(false);

            var translator = new SqlTranslator<IRecommendation>(sql, query);
            sql = translator.Translate();

            var dtos = Database.Fetch<RecommendationDto>(sql);
            return dtos.Select(dto => RecommendationFactory.BuildEntity(dto, _redactionScoreRepository.Get(dto.RecommendationStrategy)!)).ToList();
        }

        protected override void PersistNewItem(IRecommendation entity)
        {
            entity.AddingEntity();
            if (entity.Key == Guid.Empty) entity.Key = Guid.NewGuid();

            var dto = RecommendationFactory.BuildDto(entity);
            var id = Convert.ToInt32(Database.Insert(dto));

            entity.Id = id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IRecommendation entity)
        {
            entity.UpdatingEntity();

            var dto = RecommendationFactory.BuildDto(entity);
            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        private Sql<ISqlContext> SelectScore(Sql<ISqlContext> sql, RecommendationScoreParameters parameters, string? tableAlias, string? recommendationTableAlias)
        {
            return sql.Append($", (@vf * {SqlSyntax.GetFieldName<RecommendationDto>(e => e.VariableScore, tableAlias)}" +
                $" + @rf * {SqlSyntax.GetFieldName<RedactionScoreDto>(e => e.Score, "s")})" +
                $" * ({SqlSyntax.TimeFactorFunction(SqlSyntax.DaysDifference<RecommendationDto>(e => e.UpdateDate, tableAlias), "@tf")})" +
                $" AS orderscore"
                , new
                {
                    vf = parameters.VariableFactor,
                    tf = parameters.TimeFactor,
                    rf = parameters.RedactionFactor
                });
        }

        public RecommendationEntityCollection Get(uint page, uint pageSize, RecommendationScoreParameters parameters, RecommendationOrderingOptions orderingOptions, RecommendationFilterOptions filterOptions)
        {
            var sql = Sql()
                .Select<RecommendationDto>("r");

            if (orderingOptions.OrderBy == RecommendationOrderBy.Importance)
            {
                sql = SelectScore(sql, parameters, "r", "s");
            }

            sql.From<RecommendationDto>("r");

            if (filterOptions.Types?.Any() is true || orderingOptions.OrderBy == RecommendationOrderBy.Importance)
            {
                sql.LeftJoin<RedactionScoreDto>("s").On<RecommendationDto, RedactionScoreDto>((le, re) => le.RecommendationStrategy == re.Id, "r", "s");
            }

            sql.Where<RecommendationDto>(e => e.Ignore == false, "r");
            if (filterOptions.Types?.Any() is true)
            {
                sql.Where<RedactionScoreDto>(e => filterOptions.Types.Contains(e.RecommendationStrategy), "s");
            }

            if (!string.IsNullOrWhiteSpace(filterOptions.Query))
            {
                sql.Where<RecommendationDto>(e => e.Url.Contains(filterOptions.Query), "r");
            }

            sql = OrderByField(sql, orderingOptions.OrderBy, orderingOptions.Desc);

            var dtos = Database.Page<RecommendationDto>(page, pageSize, sql);
            return RecommendationEntityCollection.Create(dtos.Items.Select(dto => RecommendationFactory.BuildEntity(dto, _redactionScoreRepository.Get(dto.RecommendationStrategy)!)).ToList(), (int)dtos.TotalItems);
        }

        private Sql<ISqlContext> OrderByField(Sql<ISqlContext> sql, RecommendationOrderBy orderBy, bool desc)
        {
            if (desc)
            {
                return sql.OrderByDescending(orderBy.GetDatabaseFieldName());
            }
            return sql.OrderBy(orderBy.GetDatabaseFieldName());
        }

        public void Clear()
        {
            Database.DeleteMany<RecommendationDto>().Execute();
        }

        public Task CleanupAsync(double upperScore, RecommendationScoreParameters parameters)
        {
            var selectQuery = Sql().Select<RecommendationDto>("t", e => e.Id).From("t", s =>
            {
                s.Select<RecommendationDto>("r", e => e.Id);

                s = SelectScore(s, parameters, "r", "s");
                s
                    .From<RecommendationDto>("r")
                    .LeftJoin<RedactionScoreDto>("s")
                    .On<RecommendationDto, RedactionScoreDto>((le, re) => le.RecommendationStrategy == re.Id, "r", "s")
                    .Where<RecommendationDto>(e => e.Ignore == false, "r");
            })
                .Where("[t].[orderscore] < @upperScore", new { upperScore });

            var deleteQuery = Sql().Delete<RecommendationDto>()
                .WhereIn<RecommendationDto>(e => e.Id, selectQuery);

            return Database.ExecuteAsync(deleteQuery);
        }
    }
}
