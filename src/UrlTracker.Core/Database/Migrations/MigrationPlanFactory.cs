using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    public class MigrationPlanFactory : IMigrationPlanFactory
    {
        public MigrationPlan Create()
        {
            var result = new MigrationPlan(Defaults.DatabaseSchema.MigrationName);

            result.From(string.Empty) // add shortcut to the plan for fresh installs. This to make the install process less convoluted
                         .To<M202206251507_Rework>("2.1"); // start using new versioning system for the database

            result.From("urlTracker") // support for older db and long route if the url tracker had already been used before
                .To<M202111081155_UrlTracker>("urltracker-initial-db")
                .To<M202204091707_AddIndexes>("urltracker-add-indexes")
                .To<M202206251507_Rework>("2.0")
                .To<M202208011724_MigrateOldData>("2.1"); // introduce new versioning system for the future

            return result;
        }
    }
}
