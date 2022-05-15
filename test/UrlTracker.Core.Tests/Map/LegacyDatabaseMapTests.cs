using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Map;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests.Map
{
    public class LegacyDatabaseMapTests : TestBase
    {
        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new[]
            {
                new LegacyDatabaseMap()
            };
        }

        [TestCase(TestName = "Map UrlTrackerEntry to UrlTrackerShallowRedirect")]
        public void Map_UrlTrackerEntry_UrlTrackerShallowRedirect()
        {
            // arrange
            UrlTrackerEntry input = BaseUrlTrackerEntry;

            // act
            var result = Mapper!.Map<UrlTrackerShallowRedirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.ForceRedirect));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.PassThroughQueryString, Is.EqualTo(input.RedirectPassThroughQueryString));
                Assert.That(result.SourceRegex, Is.EqualTo(input.OldRegex));
                Assert.That(result.SourceUrl, Is.EqualTo(input.OldUrl));
                Assert.That(result.TargetNodeId, Is.EqualTo(input.RedirectNodeId));
                Assert.That(result.TargetRootNodeId, Is.EqualTo(input.RedirectRootNodeId));
                Assert.That(result.TargetStatusCode, Is.EqualTo(HttpStatusCode.MovedPermanently));
                Assert.That(result.TargetUrl, Is.EqualTo(input.RedirectUrl));
            });
        }

        [TestCase(TestName = "Map UrlTrackerEntry to UrlTrackerShallowClientError on 404")]
        public void Map_UrlTrackerEntry_UrlTrackerShallowClientError_Is404()
        {
            // arrange
            var input = BaseUrlTrackerEntry;
            input.Is404 = true;

            // act
            var result = Mapper!.Map<UrlTrackerShallowClientError>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.TargetStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [TestCase(TestName = "Map UrlTrackerEntry to UrlTrackerShallowClientError on 410")]
        public void Map_UrlTrackerEntry_UrlTrackerShallowClientError()
        {
            // arrange
            var input = BaseUrlTrackerEntry;
            input.RedirectHttpCode = 410;

            // act
            var result = Mapper!.Map<UrlTrackerShallowClientError>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.TargetStatusCode, Is.EqualTo(HttpStatusCode.Gone));
            });
        }

        [TestCase(TestName = "Map UrlTrackerEntry to UrlTrackerRedirect")]
        public void Map_UrlTrackerEntry_UrlTrackerRedirect()
        {
            // arrange
            var input = BaseUrlTrackerEntry;

            // act
            var result = Mapper!.Map<UrlTrackerRedirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                // no need to check all properties, because they are already checked by the shallow redirect test
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
            });
        }

        [TestCase(TestName = "Map UrlTrackerEntry to UrlTrackerNotFound")]
        public void Map_UrlTrackerEntry_UrlTrackerNotFound()
        {
            // arrange
            var input = BaseUrlTrackerEntry;
            input.Is404 = true;

            // act
            var result = Mapper!.Map<UrlTrackerNotFound>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Ignored, Is.False);
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Referrer, Is.EqualTo(input.Referrer));
                Assert.That(result.Url, Is.EqualTo(input.OldUrl));
            });
        }

        [TestCase(TestName = "Map UrlTrackerRedirect to UrlTrackerEntry")]
        public void Map_UrlTrackerRedirect_UrlTrackerEntry()
        {
            // arrange
            var input = new UrlTrackerRedirect
            {
                Culture = "nl-nl",
                Force = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 23),
                Notes = "lorem ipsum",
                PassThroughQueryString = true,
                SourceRegex = "dolor sit",
                SourceUrl = "http://example.com",
                TargetNodeId = 1001,
                TargetRootNodeId = 1002,
                TargetStatusCode = HttpStatusCode.Redirect,
                TargetUrl = "http://example.com/lorem"
            };

            // act
            var result = Mapper!.Map<UrlTrackerEntry>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.ForceRedirect, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.RedirectPassThroughQueryString, Is.EqualTo(input.PassThroughQueryString));
                Assert.That(result.OldRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.OldUrl, Is.EqualTo(input.SourceUrl));
                Assert.That(result.RedirectNodeId, Is.EqualTo(input.TargetNodeId));
                Assert.That(result.RedirectRootNodeId, Is.EqualTo(input.TargetRootNodeId));
                Assert.That(result.RedirectHttpCode, Is.EqualTo(302));
                Assert.That(result.RedirectUrl, Is.EqualTo(input.TargetUrl));
                Assert.That(result.Is404, Is.False);
            });
        }

        [TestCase(TestName = "Map UrlTrackerNotFound to UrlTrackerEntry")]
        public void Map_UrlTrackerNotFound_UrlTrackerEntry()
        {
            // arrange
            var input = new UrlTrackerNotFound("http://example.com/lorem")
            {
                Id = 1000,
                Ignored = false,
                Inserted = new DateTime(2022, 1, 23),
                Referrer = "http://example.com"
            };

            // act
            var result = Mapper!.Map<UrlTrackerEntry>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.Null);
                Assert.That(result.ForceRedirect, Is.False);
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Is404, Is.True);
                Assert.That(result.Notes, Is.Null);
                Assert.That(result.OldRegex, Is.Null);
                Assert.That(result.OldUrl, Is.EqualTo(input.Url));
                Assert.That(result.RedirectHttpCode, Is.Zero); // think about this. Is this expected behaviour?
                Assert.That(result.RedirectNodeId, Is.Null);
                Assert.That(result.RedirectPassThroughQueryString, Is.False);
                Assert.That(result.RedirectRootNodeId, Is.EqualTo(-1)); // think about this. Is this expected behaviour?
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.Referrer, Is.EqualTo(input.Referrer));
            });
        }

        [TestCase(TestName = "Map UrlTrackerEntryNotFoundAggregate to UrlTrackerRichNotFound")]
        public void Map_UrlTrackerEntryNotFoundAggregate_UrlTrackerRichNotFound()
        {
            // arrange
            var input = new UrlTrackerEntryNotFoundAggregate
            {
                Culture = null,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 23),
                Is404 = true,
                Occurrences = 1,
                OldUrl = "http://example.com",
                RedirectRootNodeId = null,
                Referrer = "http://example.com/lorem"
            };

            // act
            var result = Mapper!.Map<UrlTrackerRichNotFound>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.LatestOccurrence, Is.EqualTo(input.Inserted));
                Assert.That(result.MostCommonReferrer, Is.EqualTo(input.Referrer));
                Assert.That(result.Occurrences, Is.EqualTo(input.Occurrences));
                Assert.That(result.Url, Is.EqualTo(input.OldUrl));
            });
        }

        private static UrlTrackerEntry BaseUrlTrackerEntry =>
            new()
            {
                Culture = "nl-nl",
                ForceRedirect = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 23),
                Is404 = false,
                Notes = "Lorem ipsum",
                OldRegex = "dolor sit",
                OldUrl = "http://example.com",
                RedirectHttpCode = 301,
                RedirectNodeId = 1001,
                RedirectPassThroughQueryString = true,
                RedirectRootNodeId = 1002,
                RedirectUrl = "http://example.com/lorem",
                Referrer = null
            };
    }
}
