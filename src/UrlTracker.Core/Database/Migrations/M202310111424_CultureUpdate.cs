using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using UrlTracker.Core.Database.Strategy;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class M202310111424_CultureUpdate : MigrationBase
    {
        private IMigrationContext context;
        public M202310111424_CultureUpdate(IMigrationContext context) : base(context)
        {
            this.context = context;
        }

        protected override void Migrate()
        {
            var migrationStrategy = GetStrategy();
            migrationStrategy.DoMigration();
        }

        IMigrationStrategy GetStrategy()
        {
            if (Database.DatabaseType.Equals(DatabaseType.SQLite)) { return new CultureUpdateSqliteMigrationStrategy(context); }
            else { return new CultureUpdateSqlServerMigrationStrategy(context); }
        }
    }
}
