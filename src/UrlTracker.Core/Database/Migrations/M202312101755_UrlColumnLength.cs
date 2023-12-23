using System.Diagnostics.CodeAnalysis;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class M202312101755_UrlColumnLength : MigrationBase
    {
        private const int UrlMaxLength = 2083;

        private readonly IMigrationContext _context;

        public M202312101755_UrlColumnLength(IMigrationContext context)
            : base(context)
        {
            _context = context;
        }

        protected override void Migrate()
        {
            // SQLite doesn't need this upgrade
            if (_context.Database.DatabaseType is SQLiteDatabaseType)
            {
                return;
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
        }
    }
}