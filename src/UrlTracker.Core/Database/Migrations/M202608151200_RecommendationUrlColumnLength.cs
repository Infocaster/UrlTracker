using System.Diagnostics.CodeAnalysis;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    // REMINDER: Migration code is ALWAYS isolated. Do not reuse constants from shared code. Existing migrations should never change, unless they're broken.
    [ExcludeFromCodeCoverage]
    internal class M202608151200_RecommendationUrlColumnLength : MigrationBase
    {
        private const int _urlMaxLength = 2083;
        private const string _recommendationTableName = "urltrackerRecommendation";
        private const string _urlColumn = "url";

        public M202608151200_RecommendationUrlColumnLength(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            // SQLite doesn't need this upgrade
            if (DatabaseType is SQLiteDatabaseType) return;

            // M202312101755_UrlColumnLength widened the url columns on ClientError, Redirect and Referrer to fit long
            // URLs (e.g. with Google Ads tracking parameters), but missed the Recommendation table, which is populated
            // from the same client error data and therefore needs the same column length.
            Alter.Table(_recommendationTableName).AlterColumn(_urlColumn).AsString(_urlMaxLength).NotNullable().Do();
        }
    }
}
