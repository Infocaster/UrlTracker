using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202206251507_Rework
        : MigrationBase
    {
        public M202206251507_Rework(IMigrationContext context) : base(context)
        { }

        protected override void Migrate()
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
        }
    }
}
