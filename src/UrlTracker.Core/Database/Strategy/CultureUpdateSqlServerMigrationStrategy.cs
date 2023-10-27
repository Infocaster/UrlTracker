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
    public class CultureUpdateSqlServerMigrationStrategy : MigrationBase, IMigrationStrategy
    {
        public CultureUpdateSqlServerMigrationStrategy(IMigrationContext context) : base(context)
        {
        }

        public void DoMigration()
        {
            Migrate();
        }

        protected override void Migrate()
        {
            string cultureColumnName = "culture";
            if (ColumnExists(M202206251507_Rework_RedirectDto.TableName, cultureColumnName))
            {
                Alter.Table(M202206251507_Rework_RedirectDto.TableName).AlterColumn(cultureColumnName).AsString(11).Nullable().Do();
            }
        }
    }
}
