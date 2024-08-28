using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Persistence;
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
    public interface IRedactionScoreRepository
        : IReadWriteQueryRepository<Guid, IRedactionScore>, IReadRepository<int, IRedactionScore>
    { }

    internal class RedactionScoreRepository
        : EntityRepositoryBase<Guid, IRedactionScore>, IRedactionScoreRepository
    {
        public RedactionScoreRepository(IScopeAccessor scopeAccessor, AppCaches appCaches, ILogger<RedactionScoreRepository> logger)
            : base(scopeAccessor, appCaches, logger)
        { }

        public bool Exists(int id)
        {
            return GetMany(id).Any();
        }

        public IRedactionScore? Get(int id)
        {
            return GetMany(id).FirstOrDefault();
        }

        public IEnumerable<IRedactionScore> GetMany(params int[]? ids)
        {
            var all = (this as IReadRepository<Guid, IRedactionScore>).GetMany();
            if (ids?.Any() != true) return all;

            return all.Where(e => ids.Contains(e.Id));
        }

        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            var sql = Sql();
            sql = isCount
                ? sql.SelectCount()
                : sql.Select<RedactionScoreDto>();

            sql.From<RedactionScoreDto>();
            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return $"{SqlSyntax.GetFieldName<RedactionScoreDto>(e => e.RecommendationStrategy)} = @id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            return new[]
            {
                $"DELETE FROM {Defaults.DatabaseSchema.Tables.RedactionScore} WHERE {SqlSyntax.GetFieldName<RedactionScoreDto>(e => e.RecommendationStrategy)} = @id"
            };
        }

        protected override IRedactionScore? PerformGet(Guid id)
        {
            var sql = GetBaseQuery(false);
            sql.Where(GetBaseWhereClause(), new { id });

            var dto = Database.Fetch<RedactionScoreDto>(sql.SelectTop(1)).FirstOrDefault();
            if (dto is null) return null;

            return RecommendationFactory.BuildEntity(dto);
        }

        protected override IEnumerable<IRedactionScore> PerformGetAll(params Guid[]? ids)
        {
            var sql = GetBaseQuery(false);
            if (ids?.Any() is true) sql.WhereIn<RedactionScoreDto>(e => e.RecommendationStrategy, ids);

            var dtos = Database.Fetch<RedactionScoreDto>(sql);
            return dtos.Select(RecommendationFactory.BuildEntity);
        }

        protected override IEnumerable<IRedactionScore> PerformGetByQuery(IQuery<IRedactionScore> query)
        {
            var sql = GetBaseQuery(false);

            var translator = new SqlTranslator<IRedactionScore>(sql, query);
            sql = translator.Translate();

            var dtos = Database.Fetch<RedactionScoreDto>(sql);

            return dtos.Select(RecommendationFactory.BuildEntity);
        }

        protected override void PersistNewItem(IRedactionScore entity)
        {
            entity.AddingEntity();
            var dto = RecommendationFactory.BuildDto(entity);

            var id = Convert.ToInt32(Database.Insert(dto));
            entity.Id = id;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IRedactionScore entity)
        {
            entity.UpdatingEntity();
            var dto = RecommendationFactory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}
