using System;
using System.Diagnostics.CodeAnalysis;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202210291430_RecommendationModel
        : MigrationBase
    {
        internal const string _redirectTableName = "urltrackerRedirect";
        internal const string _oldSourceName = "sourceUrl";
        internal const string _newSourceName = "sourceValue";
        internal const string _sourceStrategyColumnName = "sourceStrategy";
        internal const string _oldSourceRegexStrategy = "sourceRegex";
        internal const string _oldTargetName = "targetUrl";
        internal const string _newTargetName = "targetValue";
        internal const string _targetStrategyColumnName = "targetStrategy";

        public M202210291430_RecommendationModel(IMigrationContext context)
            : base(context)
        { }

        protected override void Migrate()
        {
            MigrateRedirects();
            CreateRecommendations();
        }

        private void CreateRecommendations()
        {
            Create.Table<M202210291430_RecommendationModelSchema_RedactionScoreDto>().Do();
            Create.Table<M202210291430_RecommendationModelSchema_RecommendationDto>().Do();
        }

        private void MigrateRedirects()
        {
            var migrationHelper = GetMigrationHelper();

            migrationHelper.MigrateSource();
            migrationHelper.MigrateTarget();

            // All url source strategies should be transformed in order to work with the new approach:
            //  - Urls with root nodes need to be processed individually to append the domains to the source value
            //  - Urls with cultures should take the culture into account when appending root node domains
            // I'm not doing this here though, because the logic to perform such actions is highly complicated and time-consuming

            // All content redirects with culture should have the culture prepended to the value with a ; as separator
            Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [targetValue] = [targetValue] + ';' + [culture]
                               WHERE [culture] IS NOT NULL AND [culture] != '*' AND [targetStrategy] = @targetStrategy", new { targetStrategy = Defaults.DatabaseSchema.RedirectTargetStrategies.Content });

            Delete.Column("notes")
                  .Column("culture")
                  .Column("targetNodeId")
                  .Column("targetRootNodeId")
                  .Column("sourceRegex")
                  .FromTable(_redirectTableName)
                  .Do();
        }

        private IMigrationHelper GetMigrationHelper()
        {
            return DatabaseType switch
            {
                SQLiteDatabaseType => new SqliteMigrationHelper(this),
                _ => new DefaultMigrationHelper(this)
            };
        }

        private interface IMigrationHelper
        {
            void MigrateSource();
            void MigrateTarget();
        }

        private class DefaultMigrationHelper : IMigrationHelper
        {
            private readonly M202210291430_RecommendationModel _migration;

            public DefaultMigrationHelper(M202210291430_RecommendationModel migration)
            {
                _migration = migration;
            }

            public void MigrateSource()
            {
                _migration.Rename.Column(_oldSourceName).OnTable(_redirectTableName).To(_newSourceName).Do();
                _migration.Create.Column(_sourceStrategyColumnName).OnTable(_redirectTableName).AsGuid().NotNullable().WithDefaultValue(Defaults.DatabaseSchema.RedirectSourceStrategies.Url).Do();

                // All redirects that have no source value are regular expression redirects
                _migration.Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [sourceStrategy] = @sourceStrategy, [sourceValue] = [sourceRegex]
                               WHERE [sourceValue] IS NULL", new { sourceStrategy = Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression });

                _migration.Alter.Table(_redirectTableName).AlterColumn(_newSourceName).AsString(255).NotNullable().Do();
            }

            public void MigrateTarget()
            {
                _migration.Rename.Column(_oldTargetName).OnTable(_redirectTableName).To(_newTargetName).Do();
                _migration.Create.Column(_targetStrategyColumnName).OnTable(_redirectTableName).AsGuid().NotNullable().WithDefaultValue(Defaults.DatabaseSchema.RedirectTargetStrategies.Url).Do();

                // All redirects that have a target node id are content redirects
                _migration.Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [targetStrategy] = @targetStrategy, [targetValue] = [targetNodeId]
                               WHERE [targetNodeId] IS NOT NULL", new { targetStrategy = Defaults.DatabaseSchema.RedirectTargetStrategies.Content });
                _migration.Alter.Table(_redirectTableName).AlterColumn(_newTargetName).AsString(255).NotNullable().Do();
            }
        }

        private class SqliteMigrationHelper : IMigrationHelper
        {
            private readonly M202210291430_RecommendationModel _migration;

            public SqliteMigrationHelper(M202210291430_RecommendationModel migration)
            {
                _migration = migration;
            }

            public void MigrateSource()
            {
                _migration.Create.Column(_newSourceName).OnTable(_redirectTableName).AsString(255).NotNullable().WithDefaultValue(string.Empty).Do();
                _migration.Create.Column(_sourceStrategyColumnName).OnTable(_redirectTableName).AsGuid().NotNullable().WithDefaultValue(Defaults.DatabaseSchema.RedirectSourceStrategies.Url).Do();

                // All redirects that have no source value are regular expression redirects
                _migration.Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [sourceStrategy] = @sourceStrategy, [sourceValue] = [sourceRegex]
                               WHERE [sourceUrl] IS NULL", new { sourceStrategy = Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression });
                _migration.Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [sourceValue] = [sourceUrl]
                               WHERE [sourceUrl] IS NOT NULL");

                _migration.Delete.Column(_oldSourceName).FromTable(_redirectTableName).Do();
            }

            public void MigrateTarget()
            {
                _migration.Create.Column(_newTargetName).OnTable(_redirectTableName).AsString(255).NotNullable().WithDefaultValue(string.Empty).Do();
                _migration.Create.Column(_targetStrategyColumnName).OnTable(_redirectTableName).AsGuid().NotNullable().WithDefaultValue(Defaults.DatabaseSchema.RedirectTargetStrategies.Url).Do();

                // All redirects that have a target node id are content redirects
                _migration.Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [targetStrategy] = @targetStrategy, [targetValue] = [targetNodeId]
                               WHERE [targetNodeId] IS NOT NULL", new { targetStrategy = Defaults.DatabaseSchema.RedirectTargetStrategies.Content });
                _migration.Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [targetValue] = [targetUrl]
                               WHERE [targetUrl] IS NOT NULL");
                _migration.Delete.Column(_oldTargetName).FromTable(_redirectTableName).Do();
            }
        }
    }
}
