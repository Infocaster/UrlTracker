using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    public class M202204091707_AddIndexes : MigrationBase
    {
        private const string _indexName = "IX_" + _tableName + "_" + _cultureColumn + "_" + _oldUrlColumn + "_" + _redirectNodeIdColumn;
        private const string _tableName = "icUrlTracker";
        private const string _cultureColumn = "Culture";
        private const string _oldUrlColumn = "OldUrl";
        private const string _redirectNodeIdColumn = "RedirectNodeId";
        
        public M202204091707_AddIndexes(IMigrationContext context)
            : base(context)
        { }
        
        public override void Migrate()
        {
            Logger.LogApplyMigration<M202204091707_AddIndexes>(nameof(M202204091707_AddIndexes));
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
                Logger.LogStepSuccess<M202204091707_AddIndexes>($"Create {_indexName}");
            }
            else
            {
                Logger.LogSkipStep<M202204091707_AddIndexes>($"Create {_indexName}", "Index already exists");
            }
        }
    }
}
