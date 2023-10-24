using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;
using UrlTracker.Core.Database.Migrations;

namespace UrlTracker.Core.Database.Strategy
{
    [ExcludeFromCodeCoverage]
    public class CultureUpdateSqliteMigrationStrategy : MigrationBase, IMigrationStrategy 
    {
        public CultureUpdateSqliteMigrationStrategy(IMigrationContext context) : base(context)
        {
        }

        public void DoMigration()
        {
            Migrate();  
        }

        protected override void Migrate()
        {
            // No migration needed for Sqlite
        }
    }
}
