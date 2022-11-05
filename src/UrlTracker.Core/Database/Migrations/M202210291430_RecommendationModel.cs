using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal class M202210291430_RecommendationModel
        : MigrationBase
    {
        private const string _redirectTableName = "urltrackerRedirect";
        private const string _oldSourceName = "sourceUrl";
        private const string _newSourceName = "sourceValue";
        private const string _sourceStrategyColumnName = "sourceStrategy";
        private const string _oldSourceRegexStrategy = "sourceRegex";
        private const string _oldTargetName = "targetUrl";
        private const string _newTargetName = "targetValue";
        private const string _targetStrategyColumnName = "targetStrategy";

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
            Rename.Column(_oldSourceName).OnTable(_redirectTableName).To(_newSourceName).Do();
            Create.Column(_sourceStrategyColumnName).OnTable(_redirectTableName).AsGuid().NotNullable().WithDefaultValue(Defaults.DatabaseSchema.RedirectSourceStrategies.Url).Do();

            // All redirects that have no source value are regular expression redirects
            Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [sourceStrategy] = @sourceStrategy, [sourceValue] = [sourceRegex]
                               WHERE [sourceValue] IS NULL", new { sourceStrategy = Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression });

            Alter.Table(_redirectTableName).AlterColumn(_newSourceName).AsString(255).NotNullable().Do();

            Rename.Column(_oldTargetName).OnTable(_redirectTableName).To(_newTargetName).Do();
            Create.Column(_targetStrategyColumnName).OnTable(_redirectTableName).AsGuid().NotNullable().WithDefaultValue(Defaults.DatabaseSchema.RedirectTargetStrategies.Url).Do();

            // All redirects that have a target node id are content redirects
            Database.Execute(@"UPDATE [urltrackerRedirect]
                               SET [targetStrategy] = @targetStrategy, [targetValue] = [targetNodeId]
                               WHERE [targetNodeId] IS NOT NULL", new { targetStrategy = Defaults.DatabaseSchema.RedirectTargetStrategies.Content });
            Alter.Table(_redirectTableName).AlterColumn(_newTargetName).AsString(255).NotNullable().Do();

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
    }
}
