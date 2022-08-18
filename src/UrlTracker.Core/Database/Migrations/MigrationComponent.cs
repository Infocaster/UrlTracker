using System.Diagnostics.CodeAnalysis;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class MigrationComponent
        : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;
        private readonly IMigrationPlanFactory _migrationPlanFactory;
        private readonly ILogger _logger;

        public MigrationComponent(IScopeProvider scopeProvider,
                                  IMigrationBuilder migrationBuilder,
                                  IKeyValueService keyValueService,
                                  IRuntimeState runtimeState,
                                  IMigrationPlanFactory migrationPlanFactory,
                                  ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
            _migrationPlanFactory = migrationPlanFactory;
            _logger = logger;
        }

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
                return;

            var migrationPlan = _migrationPlanFactory.Create();

            if (migrationPlan != null)
            {
                var upgrader = new Upgrader(migrationPlan);
                upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
            }
        }

        public void Terminate()
        { }
    }
}
