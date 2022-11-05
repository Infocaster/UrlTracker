using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202210291350_DeleteOldTables
        : MigrationBase
    {
        private const string _urlTrackerTable = "icUrlTracker";
        private const string _urlTrackerNotFoundTable = "icUrlTrackerIgnore404";

        public M202210291350_DeleteOldTables(IMigrationContext context)
            : base(context)
        { }

        /// <inheritdoc />
        protected override void Migrate()
        {
            DeleteTableIfExists(_urlTrackerTable);
            DeleteTableIfExists(_urlTrackerNotFoundTable);
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
