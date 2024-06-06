using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    internal class M202406061746_PatchSqlite
        : MigrationBase
    {
        public M202406061746_PatchSqlite(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            // This fix specifically only applies to Sqlite databases
            if (DatabaseType is not SQLiteDatabaseType) return;

            Database.Execute("UPDATE `urltrackerRedirect` SET sourceStrategy=UPPER(sourceStrategy), targetStrategy=UPPER(targetStrategy)");
        }
    }
}
