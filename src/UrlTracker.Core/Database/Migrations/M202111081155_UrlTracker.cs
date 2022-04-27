using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class M202111081155_UrlTracker
        : MigrationBase
    {
        private const string _urlTrackerEntryTableName = "icUrlTracker";
        private const string _urlTrackerIgnore404TableName = "icUrlTrackerIgnore404";

        public M202111081155_UrlTracker(IMigrationContext context)
            : base(context)
        { }

        public override void Migrate()
        {
            Logger.LogApplyMigration<M202111081155_UrlTracker>(nameof(M202111081155_UrlTracker));

            if (!TableExists(_urlTrackerEntryTableName))
            {
                Create.Table<M202111081155_UrlTrackerSchema_IcUrlTracker>().Do();
                Logger.LogStepSuccess<M202111081155_UrlTracker>($"Create {_urlTrackerEntryTableName}");
            }
            else
            {
                Logger.LogSkipStep<M202111081155_UrlTracker>($"Create {_urlTrackerEntryTableName}", "Table already exists");
            }

            if (!TableExists(_urlTrackerIgnore404TableName))
            {
                Create.Table<M202111081155_UrlTrackerSchema_IcUrlTrackerIgnore404>().Do();
                Logger.LogStepSuccess<M202111081155_UrlTracker>($"Create {_urlTrackerIgnore404TableName}");
            }
            else
            {
                Logger.LogSkipStep<M202111081155_UrlTracker>($"Create {_urlTrackerIgnore404TableName}", "Table already exists");
            }
        }
    }
}
