using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

namespace UrlTracker.Core.Database
{
    [ExcludeFromCodeCoverage]
    public class ReferrerRepository
        : EntityRepositoryBase<int, IReferrer>, IReferrerRepository
    {
        public ReferrerRepository(IScopeAccessor scopeAccessor, AppCaches appCaches, ILogger<EntityRepositoryBase<int, IReferrer>> logger)
            : base(scopeAccessor, appCaches, logger)
        { }

        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            var sql = isCount
                ? Sql().SelectCount()
                : Sql().Select<ReferrerDto>();

            sql.From<ReferrerDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "id = @id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            return new List<string>
            {
                $"DELETE FROM {Defaults.DatabaseSchema.Tables.Referrer} WHERE id = @id"
            };
        }

        protected override IReferrer? PerformGet(int id)
        {
            var sql = GetBaseQuery(false);
            sql.Where<ReferrerDto>(e => e.Id == id);

            var dto = Database.Fetch<ReferrerDto>(sql).FirstOrDefault();
            if (dto is null) return null;

            return ClientErrorFactory.BuildEntity(dto);
        }

        protected override IEnumerable<IReferrer> PerformGetAll(params int[]? ids)
        {
            var sql = GetBaseQuery(false);
            if (ids?.Any() is true) sql.WhereIn<ReferrerDto>(e => e.Id, ids);

            var dtos = Database.Fetch<ReferrerDto>(sql);

            return dtos.Select(ClientErrorFactory.BuildEntity);
        }

        protected override IEnumerable<IReferrer> PerformGetByQuery(IQuery<IReferrer> query)
        {
            var sql = GetBaseQuery(false);

            var translator = new SqlTranslator<IReferrer>(sql, query);
            sql = translator.Translate();

            var dtos = Database.Fetch<ReferrerDto>(sql);
            return dtos.Select(ClientErrorFactory.BuildEntity);
        }

        protected override void PersistNewItem(IReferrer entity)
        {
            entity.AddingEntity();
            if (entity.Key == Guid.Empty) entity.Key = Guid.NewGuid();

            var dto = ClientErrorFactory.BuildDto(entity);
            var id = Convert.ToInt32(Database.Insert(dto));

            entity.Id = id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IReferrer entity)
        {
            entity.UpdatingEntity();

            var dto = ClientErrorFactory.BuildDto(entity);
            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        public IReferrer? Get(string url)
        {
            return Get(SqlContext.Query<IReferrer>().Where(e => e.Url == url)).FirstOrDefault();
        }

        public IEnumerable<IReferrer?> GetAll(string url)
        {
            return Get(SqlContext.Query<IReferrer>().Where(e => e.Url == url));
        }
    }
}
