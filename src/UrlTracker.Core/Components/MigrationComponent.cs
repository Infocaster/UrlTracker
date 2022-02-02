using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Composing;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using UrlTracker.Core.Database.Migrations;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Core.Components
{
    [ExcludeFromCodeCoverage]
    public class MigrationComponent
        : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger _logger;

        public MigrationComponent(IScopeProvider scopeProvider,
                                  IMigrationBuilder migrationBuilder,
                                  IKeyValueService keyValueService,
                                  ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }

        public void Initialize()
        {
            var migrationPlan = new MigrationPlan("UrlTracker");
            migrationPlan.From(string.Empty)
                .To("urlTracker") // Add empty migration to support the original database that didn't use migrations as they were supposed to.
                .To<M202111081155_UrlTracker>("urltracker-initial-db");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        { }
    }
}
