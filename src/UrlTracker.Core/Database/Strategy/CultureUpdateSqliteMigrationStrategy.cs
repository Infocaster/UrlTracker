using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;
using UrlTracker.Core.Database.Migrations;

namespace UrlTracker.Core.Database.Strategy
{
    [ExcludeFromCodeCoverage]
    public class CultureUpdateSqliteMigrationStrategy : AsyncMigrationBase, IMigrationStrategy 
    {
        public CultureUpdateSqliteMigrationStrategy(IMigrationContext context) : base(context)
        {
        }

        public Task DoMigrationAsync()
        {
            return MigrateAsync();
        }

        protected override Task MigrateAsync()
        {
            // No migration needed for Sqlite
            return Task.CompletedTask;
        }
    }
}
