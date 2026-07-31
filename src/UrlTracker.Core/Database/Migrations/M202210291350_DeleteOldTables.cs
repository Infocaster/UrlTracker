using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202210291350_DeleteOldTables
        : AsyncMigrationBase
    {
        private const string _urlTrackerTable = "icUrlTracker";
        private const string _urlTrackerNotFoundTable = "icUrlTrackerIgnore404";

        public M202210291350_DeleteOldTables(IMigrationContext context)
            : base(context)
        { }

        /// <inheritdoc />
        protected override Task MigrateAsync()
        {
            DeleteTableIfExists(_urlTrackerTable);
            DeleteTableIfExists(_urlTrackerNotFoundTable);
            return Task.CompletedTask;
        }

        private void DeleteTableIfExists(string tableName)
        {
            if (TableExists(tableName))
            {
                Delete.Table(tableName).Do();
            }
        }
    }
}
