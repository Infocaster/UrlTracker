using Umbraco.Core.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    public interface IMigrationPlanFactory
    {
        MigrationPlan Create();
    }
}