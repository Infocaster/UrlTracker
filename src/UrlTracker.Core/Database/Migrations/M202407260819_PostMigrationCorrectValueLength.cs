using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;
using static Umbraco.Cms.Core.Constants.Conventions;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202407260819_PostMigrationCorrectValueLength
        : AsyncMigrationBase
    {
        private const int _urlMaxLength = 2083;
        private const string _redirectTableName = "urltrackerRedirect";
        private const string _sourceValueColumn = "sourceValue";
        private const string _targetValueColumn = "targetValue";

        public M202407260819_PostMigrationCorrectValueLength(IMigrationContext context)
            : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            // This migration is specifically for SQL Server, because Sqlite doesn't have explicit sizes
            if (DatabaseType is SQLiteDatabaseType) return Task.CompletedTask;

            // In a previous version of the URL Tracker, there was an error that caused migrations to fail, because the strategy value columns were shortened
            // That migration is patched in this version, but additionally, we need to make sure that the columns are corrected for users who used the previous version and might now have the wrong column length
            Alter.Table(_redirectTableName).AlterColumn(_sourceValueColumn).AsString(_urlMaxLength).NotNullable().Do();
            Alter.Table(_redirectTableName).AlterColumn(_targetValueColumn).AsString(_urlMaxLength).NotNullable().Do();
            
            return Task.CompletedTask;
        }
    }
}
