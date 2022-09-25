using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace UrlTracker.Core.Database.Migrations
{
    /// <summary>
    /// A component that performs the database migration for the URL Tracker
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MigrationComponent
        : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;
        private readonly IMigrationPlanFactory _migrationPlanFactory;

        /// <summary>
        /// Create an instance of the MigrationComponent using dependency injection
        /// </summary>
        /// <param name="scopeProvider">The scope provider</param>
        /// <param name="migrationPlanExecutor">The migration plan executor</param>
        /// <param name="keyValueService">The key value service</param>
        /// <param name="runtimeState">The runtime state</param>
        /// <param name="migrationPlanFactory">The migration plan factory</param>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Terminate()
        { }
    }
}
