using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;
using UrlTracker.Core.Database.Migrations;

namespace UrlTracker.Core.Database.Strategy
{
    [ExcludeFromCodeCoverage]
    public class CultureUpdateSqlServerMigrationStrategy : AsyncMigrationBase, IMigrationStrategy
    {
        public CultureUpdateSqlServerMigrationStrategy(IMigrationContext context) : base(context)
        {
        }

        public Task DoMigrationAsync()
        {
            return MigrateAsync();
        }

        protected override Task MigrateAsync()
        {
            string cultureColumnName = "culture";
            if (ColumnExists(M202206251507_Rework_RedirectDto.TableName, cultureColumnName))
            {
                Alter.Table(M202206251507_Rework_RedirectDto.TableName).AlterColumn(cultureColumnName).AsString(11).Nullable().Do();
            }
            return Task.CompletedTask;
        }
    }
}
