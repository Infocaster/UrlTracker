using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using UrlTracker.Core.Database.Strategy;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class M202310111424_CultureUpdate : AsyncMigrationBase
    {
        private IMigrationContext context;
        public M202310111424_CultureUpdate(IMigrationContext context) : base(context)
        {
            this.context = context;
        }

        protected override Task MigrateAsync()
        {
            var migrationStrategy = GetStrategy();
            return migrationStrategy.DoMigrationAsync();
        }

        IMigrationStrategy GetStrategy()
        {
            if (Database.DatabaseType.Equals(DatabaseType.SQLite)) { return new CultureUpdateSqliteMigrationStrategy(context); }
            else { return new CultureUpdateSqlServerMigrationStrategy(context); }
        }
    }
}
