using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class M202312101755_UrlColumnLength : AsyncMigrationBase
    {
        private const int UrlMaxLength = 2083;

        private readonly IMigrationContext _context;

        public M202312101755_UrlColumnLength(IMigrationContext context)
            : base(context)
        {
            _context = context;
        }

        protected override Task MigrateAsync()
        {
            // SQLite doesn't need this upgrade
            if (_context.Database.DatabaseType is SQLiteDatabaseType)
            {
                return Task.CompletedTask;
            }

            Alter.Table(Defaults.DatabaseSchema.Tables.ClientError)
                .AlterColumn("url").AsString(UrlMaxLength).NotNullable()
                    .Do();

            Alter.Table(Defaults.DatabaseSchema.Tables.Redirect)
                .AlterColumn("sourceUrl").AsString(UrlMaxLength).Nullable()
                .AlterColumn("targetUrl").AsString(UrlMaxLength).Nullable()
                    .Do();

            Alter.Table(Defaults.DatabaseSchema.Tables.Referrer)
                .AlterColumn("url").AsString(UrlMaxLength).NotNullable()
                    .Do();
            
            return Task.CompletedTask;
        }
    }
}