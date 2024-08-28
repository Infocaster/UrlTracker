using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202204091707_AddIndexes
        : MigrationBase
    {
        private const string _indexName = "IX_" + _tableName + "_" + _cultureColumn + "_" + _oldUrlColumn + "_" + _redirectNodeIdColumn;
        private const string _tableName = "icUrlTracker";
        private const string _cultureColumn = "Culture";
        private const string _oldUrlColumn = "OldUrl";
        private const string _redirectNodeIdColumn = "RedirectNodeId";

        public M202204091707_AddIndexes(IMigrationContext context)
            : base(context)
        { }

        protected override void Migrate()
        {
            Logger.LogApplyMigration(nameof(M202204091707_AddIndexes));

            if (!IndexExists(_indexName))
            {
                Create.Index(_indexName)
                      .OnTable(_tableName)
                      .OnColumn(_cultureColumn)
                      .Ascending()
                      .OnColumn(_oldUrlColumn)
                      .Ascending()
                      .OnColumn(_redirectNodeIdColumn)
                      .Ascending()
                      .WithOptions()
                      .NonClustered()
                      .Do();
                Logger.LogStepSuccess($"Create {_indexName}");
            }
            else
            {
                Logger.LogSkipStep($"Create {_indexName}", "Index already exists");
            }
        }
    }
}
