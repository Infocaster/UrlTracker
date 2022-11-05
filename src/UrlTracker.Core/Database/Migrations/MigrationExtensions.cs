using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence;

namespace UrlTracker.Core.Database.Migrations
{
    [ExcludeFromCodeCoverage]
    internal static class MigrationExtensions
    {
        internal static void BatchFetchAndProcess<T>(this IUmbracoDatabase database, Sql sql, Action<IEnumerable<T>> action)
        {
            long page = 1;
            long totalPages;
            do
            {
                /* Records are processed in batches of 500.000
                 * Several tests concluded that at this number the speed is still acceptable (approximately 3 minutes on 2.000.000 records)
                 * and the database doesn't time out.
                 */
                var results = database.Page<T>(page, 500000, sql);
                totalPages = results.TotalPages;
                page++;

                action(results.Items);
            }
            while (page <= totalPages);
        }
    }
}
