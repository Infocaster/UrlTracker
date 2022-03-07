using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Map
{
    public class ServiceLayerMapsTests : TestBase
    {
        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new[]
            {
                new ServiceLayerMaps(UmbracoContextFactoryAbstractionMock.UmbracoContextFactory)
            };
        }

        public override void SetUp()
        {
            UmbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetContentById(It.IsAny<int>())).Returns((int id) => new TestPublishedContent { Id = id });
        }

        [TestCase(TestName = "Map UrlTrackerShallowRedirect to ShallowRedirect")]
        public void Map_UrlTrackerShallowRedirect_ShallowRedirect()
        {
            // arrange
            var input = new UrlTrackerShallowRedirect
            {
                Culture = "nl-nl",
                Force = true,
                Id = 1000,
                PassThroughQueryString = true,
                SourceRegex = "lorem ipsum",
                SourceUrl = "http://example.com",
                TargetNodeId = 1001,
                TargetRootNodeId = 1002,
                TargetStatusCode = HttpStatusCode.Redirect,
                TargetUrl = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<ShallowRedirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.PassThroughQueryString, Is.EqualTo(input.PassThroughQueryString));
                Assert.That(result.SourceRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.SourceUrl, Is.EqualTo("http://example.com"));
                Assert.That(result.TargetNode, Is.Not.Null);
                Assert.That(result.TargetRootNode, Is.Not.Null);
                Assert.That(result.TargetStatusCode, Is.EqualTo(input.TargetStatusCode));
                Assert.That(result.TargetUrl, Is.EqualTo(input.TargetUrl));
            });
        }

        [TestCase(TestName = "Map UrlTrackerRedirect to Redirect")]
        public void Map_UrlTrackerRedirect_Redirect()
        {
            // arrange
            var input = new UrlTrackerRedirect
            {
                Inserted = new DateTime(2022, 1, 23),
                Notes = "lorem ipsum"
            };

            // act
            var result = Mapper.Map<Redirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
            });
        }

        [TestCase(TestName = "Map UrlTrackerRedirectCollection to RedirectCollection")]
        public void Map_UrlTrackerRedirectCollection_RedirectCollection()
        {
            // arrange
            var input = UrlTrackerRedirectCollection.Create(new[] { new UrlTrackerRedirect() }, 3);

            // act
            var result = Mapper.Map<RedirectCollection>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(input.Total));
                Assert.That(result.Count(), Is.EqualTo(input.Count()));
                Assert.That(result, Has.No.Null);
            });
        }

        [TestCase(TestName = "Map Redirect to UrlTrackerRedirect with content")]
        public void Map_Redirect_UrlTrackerRedirect_Content()
        {
            // arrange
            var input = new Redirect
            {
                Culture = "nl-nl",
                Force = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 23),
                Notes = "lorem ipsum",
                PassThroughQueryString = true,
                SourceRegex = "dolor sit",
                SourceUrl = "http://example.com",
                TargetNode = new TestPublishedContent { Id = 1001 },
                TargetRootNode = new TestPublishedContent { Id = 1002 },
                TargetStatusCode = HttpStatusCode.Redirect,
                TargetUrl = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<UrlTrackerRedirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.PassThroughQueryString, Is.EqualTo(input.PassThroughQueryString));
                Assert.That(result.SourceRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.SourceUrl, Is.EqualTo(input.SourceUrl));
                Assert.That(result.TargetNodeId, Is.EqualTo(input.TargetNode.Id));
                Assert.That(result.TargetRootNodeId, Is.EqualTo(input.TargetRootNode.Id));
                Assert.That(result.TargetStatusCode, Is.EqualTo(input.TargetStatusCode));
                Assert.That(result.TargetUrl, Is.EqualTo(input.TargetUrl));
            });
        }

        [TestCase(TestName = "Map Redirect to UrlTrackerRedirect without content")]
        public void Map_Redirect_UrlTrackerRedirect_NoContent()
        {
            // arrange
            var input = new Redirect();

            // act
            var result = Mapper.Map<UrlTrackerRedirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SourceUrl, Is.Null);
                Assert.That(result.TargetNodeId, Is.Null);
                Assert.That(result.TargetRootNodeId, Is.Null);
            });
        }

        [TestCase(TestName = "Map NotFound to UrlTrackerNotFound")]
        public void Map_NotFound_UrlTrackerNotFound()
        {
            // arrange
            var input = new NotFound
            {
                Id = 1000,
                Ignored = false,
                Inserted = new DateTime(2022, 1, 23),
                Referrer = "http://example.com",
                Url = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<UrlTrackerNotFound>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Ignored, Is.EqualTo(input.Ignored));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Referrer, Is.EqualTo(input.Referrer));
                Assert.That(result.Url, Is.EqualTo(input.Url));
            });
        }

        [TestCase(TestName = "Map UrlTrackerNotFound to NotFound")]
        public void Map_UrlTrackerNotFound_NotFound()
        {
            // arrange
            var input = new UrlTrackerNotFound
            {
                Id = 1000,
                Ignored = false,
                Inserted = new DateTime(2022, 1, 23),
                Referrer = "http://example.com",
                Url = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<NotFound>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Ignored, Is.EqualTo(input.Ignored));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Referrer, Is.EqualTo(input.Referrer));
                Assert.That(result.Url, Is.EqualTo(input.Url));
            });
        }

        [TestCase(TestName = "Map UrlTrackerRichNotFoundCollection to RichNotFoundCollection")]
        public void Map_UrlTrackerRichNotFoundCollection_RichNotFoundCollection()
        {
            // arrange
            var input = UrlTrackerRichNotFoundCollection.Create(new[] { new UrlTrackerRichNotFound() }, 3);

            // act
            var result = Mapper.Map<RichNotFoundCollection>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(input.Total));
                Assert.That(result.Count(), Is.EqualTo(input.Count()));
                Assert.That(result, Has.No.Null);
            });
        }

        [TestCase(TestName = "Map UrlTrackerRichNotFound to RichNotFound")]
        public void Map_UrlTrackerRichNotFound_RichNotFound()
        {
            // arrange
            var input = new UrlTrackerRichNotFound
            {
                Id = 1000,
                LatestOccurrence = new DateTime(2022, 1, 23),
                MostCommonReferrer = "http://example.com",
                Occurrences = 3,
                Url = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<RichNotFound>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.LatestOccurrence, Is.EqualTo(input.LatestOccurrence));
                Assert.That(result.MostCommonReferrer, Is.EqualTo(input.MostCommonReferrer));
                Assert.That(result.Occurrences, Is.EqualTo(input.Occurrences));
                Assert.That(result.Url, Is.EqualTo(input.Url));
            });
        }
    }
}
