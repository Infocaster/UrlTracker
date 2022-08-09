using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    public interface IMigrationPlanFactory
    {
        MigrationPlan? Create();
    }
}