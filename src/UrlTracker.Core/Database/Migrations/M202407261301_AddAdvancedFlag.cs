using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extensions;

namespace UrlTracker.Core.Database.Migrations
{
    internal partial class M202407261301_AddAdvancedFlag
        : AsyncMigrationBase
    {
        // REMINDER: Migration code is ALWAYS isolated. Do not reuse constants from shared code. Existing migrations should never change, unless they're broken.
        private const string _redirectTableName = "UrlTrackerRedirect";
        private const string _advancedFlagColumn = "advanced";
        private static readonly Guid _regexSourceStrategy = new("6dd69e53-b8dc-4a4e-84fb-e76c758bc8a5");

        public M202407261301_AddAdvancedFlag(IMigrationContext context)
            : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            AddColumn<RedirectDto>(_advancedFlagColumn);

            // At this point, all redirects are marked as regular, so we need to update all redirects that are non-regular
            // A redirect is considered advanced if it is a regex redirect or any of the advanced options have a non-default value
            // After this migration, we stop differentiating by features
            var query = Sql()
                .Update<RedirectDto>(upd => upd.Set(e => e.Advanced, true))
                .WhereAny(
                    sql => sql.Where<RedirectDto>(e => e.SourceStrategy == _regexSourceStrategy),
                    sql => sql.Where<RedirectDto>(e => e.Force == true),
                    sql => sql.Where<RedirectDto>(e => e.RetainQuery == false));

            Database.Execute(query);
            return Task.CompletedTask;
        }
    }
}
