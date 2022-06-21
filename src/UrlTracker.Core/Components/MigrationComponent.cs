using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database.Migrations;

namespace UrlTracker.Core.Components
{
    [ExcludeFromCodeCoverage]
    public class MigrationComponent
        : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public MigrationComponent(IScopeProvider scopeProvider,
                                  IMigrationPlanExecutor migrationPlanExecutor,
                                  IKeyValueService keyValueService,
                                  IRuntimeState runtimeState)
        {
            _scopeProvider = scopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run) return;

            var migrationPlan = new MigrationPlan("UrlTracker");
            migrationPlan.From(string.Empty)
                .To("urlTracker") // Add empty migration to support the original database that didn't use migrations as they were supposed to.
                .To<M202111081155_UrlTracker>("urltracker-initial-db")
                .To<M202204091707_AddIndexes>("urltracker-add-indexes");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }

        public void Terminate()
        { }
    }
}
