using System.Diagnostics;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using ThrowawayDb;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database.Migrations;
using UrlTracker.IntegrationTests.Utils;

namespace UrlTracker.IntegrationTests.Migration
{
    internal class NewTableStructureMigrationTests : IntegrationTestBase
    {
        protected override UrlTrackerWebApplicationFactory CreateApplicationFactory(ThrowawayDatabase database)
        {
            return new UrlTrackerMigrationWebApplicationFactory(database);
        }

        [TestCase(TestName = "Migration can transfer 2 000 000 records")]
        public void Migration_TwoMillionRecords_TransfersToNewTableStructure()
        {
            var migrationPlan = new MigrationPlan(Core.Defaults.DatabaseSchema.MigrationName)
                .From(string.Empty)
                .To<M202111081155_UrlTracker>("urltracker-initial-db")
                .To<M202204091707_AddIndexes>("urltracker-add-indexes")
                .To<M202206251507_Rework>("2.0");
            PerformMigration(migrationPlan);

            {
                // insert 2 million records
                var urltrackergenerator = CreateOldUrlTrackerRowGenerator();
                var ignoregenerator = CreateOldUrlTrackerIgnoreGenerator();
                var entries = urltrackergenerator.GenerateLazy(2500, "default, redirect, oldUrl, targetNode")
                    .Concat(urltrackergenerator.GenerateLazy(2500, "default, redirect, oldUrl, targetUrl"))
                    .Concat(urltrackergenerator.GenerateLazy(2500, "default, redirect, oldRegex, targetNode"))
                    .Concat(urltrackergenerator.GenerateLazy(2500, "default, redirect, oldRegex, targetUrl"))
                    .Concat(urltrackergenerator.GenerateLazy(1000000, "default, notFound"))
                    .Concat(urltrackergenerator.GenerateLazy(1000000, "default, notFound, referrer"));

                var ignoreEntries = ignoregenerator.GenerateLazy(20000);

                using (var scope = ServiceProvider.GetRequiredService<IScopeProvider>().CreateScope())
                {
                    scope.Database.BulkInsertRecords(entries);
                    scope.Database.BulkInsertRecords(ignoreEntries);

                    scope.Complete();
                }
            }

            // perform new migration!
            migrationPlan = new MigrationPlan(Core.Defaults.DatabaseSchema.MigrationName)
                .From("2.0")
                .To<M202208011724_MigrateOldData>("2.1");

            Stopwatch timer = new();
            timer.Start();
            PerformMigration(migrationPlan);
            timer.Stop();
            Console.WriteLine("Migration time: {0}", timer.Elapsed);
        }

        private void PerformMigration(MigrationPlan migrationPlan)
        {
            var migrationPlanExecutor = ServiceProvider.GetRequiredService<IMigrationPlanExecutor>();
            var scopeProvider = ServiceProvider.GetRequiredService<IScopeProvider>();
            var keyValueService = ServiceProvider.GetRequiredService<IKeyValueService>();
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(migrationPlanExecutor, scopeProvider, keyValueService);
        }

        private Faker<M202111081155_UrlTrackerSchema_IcUrlTracker> CreateOldUrlTrackerRowGenerator()
            => new Faker<M202111081155_UrlTrackerSchema_IcUrlTracker>()
            .RuleFor(p => p.Inserted, f => f.Date.Between(new DateTime(1970, 1, 1), DateTime.Now))
            .RuleFor(p => p.RedirectRootNodeId, f => f.Random.Int(1000, 9999))
            .RuleSet("notFound", r
                => r.RuleFor(p => p.OldUrl, f => f.Internet.UrlWithPath())
                    .RuleFor(p => p.Is404, true))
            .RuleSet("referrer", r
                => r.RuleFor(p => p.Referrer, f => f.Internet.UrlWithPath()))
            .RuleSet("redirect", r
                => r.RuleFor(p => p.Is404, false)
                    .RuleFor(p => p.Culture, "en-US")
                    .RuleFor(p => p.ForceRedirect, f => f.Random.Bool())
                    .RuleFor(p => p.Notes, f => string.Join(" ", f.Lorem.Words(f.Random.Number(7))))
                    .RuleFor(p => p.RedirectHttpCode, f => f.PickRandomParam(301, 302))
                    .RuleFor(p => p.RedirectPassThroughQueryString, f => f.Random.Bool()))
            .RuleSet("oldRegex", r
                => r.RuleFor(p => p.OldRegex, f => f.Random.Utf16String(2, 20)))
            .RuleSet("oldUrl", r
                => r.RuleFor(p => p.OldUrl, f => f.Random.Bool() ? f.Internet.UrlWithPath() : f.Internet.UrlRootedPath()))
            .RuleSet("targetNode", r
                => r.RuleFor(p => p.RedirectNodeId, f => f.Random.Int(1000, 9999)))
            .RuleSet("targetUrl", r
                => r.RuleFor(p => p.RedirectUrl, f => f.Random.Bool() ? f.Internet.UrlWithPath() : f.Internet.UrlRootedPath()));

        private Faker<M202111081155_UrlTrackerSchema_IcUrlTrackerIgnore404> CreateOldUrlTrackerIgnoreGenerator()
            => new Faker<M202111081155_UrlTrackerSchema_IcUrlTrackerIgnore404>()
            .RuleFor(p => p.Inserted, f => f.Date.Between(new DateTime(1970, 1, 1), DateTime.Now))
            .RuleFor(p => p.Url, f => f.Internet.UrlWithPath());
    }
}
