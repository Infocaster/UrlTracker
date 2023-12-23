using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class MigrationPlanFactory : IMigrationPlanFactory
    {
        public MigrationPlan Create()
        {
            var result = new MigrationPlan(Defaults.DatabaseSchema.MigrationName);

            result.From(string.Empty) // add shortcut to the plan for fresh installs. This to make the install process less convoluted
                .To<M202206251507_Rework>("2.1") // start using new versioning system for the database
                .To<M202310111424_CultureUpdate>("2.2")
                .To<M202312101755_UrlColumnLength>("2.3")
                .To<M202210291350_DeleteOldTables>("2.4")
                .To<M202210291430_RecommendationModel>("3.0")
                .To<M202212111209_PopulateRedactionScores>("3.1");
            // Use ☝️ this path for new migrations

            result.From("urlTracker") // support for older db and long route if the url tracker had already been used before
                  .To<M202111081155_UrlTracker>("urltracker-initial-db")
                  .To<M202204091707_AddIndexes>("urltracker-add-indexes")
                  .To<M202206251507_Rework>("2.0")
                  .To<M202208011724_MigrateOldData>("2.1"); // introduce new versioning system for the future

            return result;
        }
    }
}
