using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;
using System.Threading.Tasks;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202111081155_UrlTracker
        : AsyncMigrationBase
    {
        private const string _urlTrackerEntryTableName = "icUrlTracker";
        private const string _urlTrackerIgnore404TableName = "icUrlTrackerIgnore404";

        public M202111081155_UrlTracker(IMigrationContext context)
            : base(context)
        { }

        protected override Task MigrateAsync()
        {
            Logger.LogApplyMigration(nameof(M202111081155_UrlTracker));

            if (!TableExists(_urlTrackerEntryTableName))
            {
                Create.Table<M202111081155_UrlTrackerSchema_IcUrlTracker>().Do();
                Logger.LogStepSuccess($"Create {_urlTrackerEntryTableName}");
            }
            else
            {
                Logger.LogSkipStep($"Create {_urlTrackerEntryTableName}", "Table already exists");
            }

            if (!TableExists(_urlTrackerIgnore404TableName))
            {
                Create.Table<M202111081155_UrlTrackerSchema_IcUrlTrackerIgnore404>().Do();
                Logger.LogStepSuccess($"Create {_urlTrackerIgnore404TableName}");
            }
            else
            {
                Logger.LogSkipStep($"Create {_urlTrackerIgnore404TableName}", "Table already exists");
            }
            return Task.CompletedTask;
        }
    }
}
