using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class MigrationComponent
        : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;
        private readonly IMigrationPlanFactory _migrationPlanFactory;

        public MigrationComponent(IScopeProvider scopeProvider,
                                  IMigrationPlanExecutor migrationPlanExecutor,
                                  IKeyValueService keyValueService,
                                  IRuntimeState runtimeState,
                                  IMigrationPlanFactory migrationPlanFactory)
        {
            _scopeProvider = scopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
            _migrationPlanFactory = migrationPlanFactory;
        }

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return;

            var migrationPlan = _migrationPlanFactory.Create();

            if (migrationPlan is not null)
            {
                var upgrader = new Upgrader(migrationPlan);
                upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
            }
        }

        public void Terminate()
        { }
    }
}
