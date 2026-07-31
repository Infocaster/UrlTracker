using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202206251507_Rework
        : AsyncMigrationBase
    {
        public M202206251507_Rework(IMigrationContext context) : base(context)
        { }

        protected override Task MigrateAsync()
        {
            if (!TableExists(M202206251507_Rework_RedirectDto.TableName))
            {
                Create.Table<M202206251507_Rework_RedirectDto>().Do();
            }

            if (!TableExists(M202206251507_Rework_ClientErrorDto.TableName))
            {
                Create.Table<M202206251507_Rework_ClientErrorDto>().Do();
            }

            if (!TableExists(M202206251507_Rework_ReferrerDto.TableName))
            {
                Create.Table<M202206251507_Rework_ReferrerDto>().Do();
            }

            if (!TableExists(M202206251507_Rework_ClientError2ReferrerDto.TableName))
            {
                Create.Table<M202206251507_Rework_ClientError2ReferrerDto>().Do();
            }

            return Task.CompletedTask;
        }
    }
}
