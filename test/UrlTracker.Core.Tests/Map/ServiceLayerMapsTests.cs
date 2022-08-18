using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models.Entities;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
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
            UmbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetContentById(It.IsAny<int>())).Returns((int id) => TestPublishedContent.Create(id));
        }

        [TestCase(TestName = "Map IRedirect to Redirect")]
        public void Map_UrlTrackerRedirect_Redirect()
        {
            // arrange
            IRedirect input = new RedirectEntity(default, default, default, default, default, default, default, default, default, default)
            {
                CreateDate = new DateTime(2022, 1, 23),
                Notes = "lorem ipsum",
                Culture = "nl-nl",
                Force = true,
                Id = 1000,
                RetainQuery = true,
                SourceRegex = "lorem ipsum",
                SourceUrl = "http://example.com",
                TargetNodeId = 1001,
                TargetRootNodeId = 1002,
                Permanent = false,
                TargetUrl = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<Redirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.RetainQuery, Is.EqualTo(input.RetainQuery));
                Assert.That(result.SourceRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.SourceUrl, Is.EqualTo("http://example.com"));
                Assert.That(result.TargetNode, Is.Not.Null);
                Assert.That(result.TargetRootNode, Is.Not.Null);
                Assert.That(result.TargetStatusCode, Is.EqualTo(HttpStatusCode.Redirect));
                Assert.That(result.TargetUrl, Is.EqualTo(input.TargetUrl));
                Assert.That(result.Inserted, Is.EqualTo(input.CreateDate));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
            });
        }

        [TestCase(TestName = "Map UrlTrackerRedirectCollection to RedirectCollection")]
        public void Map_UrlTrackerRedirectCollection_RedirectCollection()
        {
            // arrange
            var input = Database.Entities.RedirectEntityCollection.Create(new[] { new RedirectEntity(default, default, default, default, default, default, default, default, default, default) }, 3);

            // act
            var result = Mapper.Map<Core.Models.RedirectCollection>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(input.Total));
                Assert.That(result.Count(), Is.EqualTo(input.Count()));
                Assert.That(result, Has.No.Null);
            });
        }

        [TestCase(TestName = "Map Redirect to IRedirect with content")]
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
                RetainQuery = true,
                SourceRegex = "dolor sit",
                SourceUrl = "http://example.com",
                TargetNode = TestPublishedContent.Create(1001),
                TargetRootNode = TestPublishedContent.Create(1002),
                TargetStatusCode = HttpStatusCode.Redirect,
                TargetUrl = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<IRedirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.CreateDate, Is.EqualTo(input.Inserted));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.RetainQuery, Is.EqualTo(input.RetainQuery));
                Assert.That(result.SourceRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.SourceUrl, Is.EqualTo(input.SourceUrl));
                Assert.That(result.TargetNodeId, Is.EqualTo(input.TargetNode.Id));
                Assert.That(result.TargetRootNodeId, Is.EqualTo(input.TargetRootNode.Id));
                Assert.That(result.Permanent, Is.False);
                Assert.That(result.TargetUrl, Is.EqualTo(input.TargetUrl));
            });
        }

        [TestCase(TestName = "Map Redirect to IRedirect without content")]
        public void Map_Redirect_UrlTrackerRedirect_NoContent()
        {
            // arrange
            var input = new Redirect();

            // act
            var result = Mapper.Map<IRedirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SourceUrl, Is.Null);
                Assert.That(result.TargetNodeId, Is.Null);
                Assert.That(result.TargetRootNodeId, Is.Null);
            });
        }

        [TestCase(TestName = "Map ClientError to UrlTrackerClientError")]
        public void Map_ClientError_UrlTrackerClientError()
        {
            // arrange
            var input = new ClientError("http://example.com/lorem")
            {
                Id = 1000,
                Ignored = false,
                Inserted = new DateTime(2022, 1, 23)
            };

            // act
            var result = Mapper.Map<IClientError>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Ignored, Is.EqualTo(input.Ignored));
                Assert.That(result.CreateDate, Is.EqualTo(input.Inserted));
                Assert.That(result.Url, Is.EqualTo(input.Url));
            });
        }

        [TestCase(TestName = "Map UrlTrackerClientError to ClientError")]
        public void Map_UrlTrackerClientError_ClientError()
        {
            // arrange
            var input = new ClientErrorEntity("http://example.com/lorem", false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound, default, "http://example.com", default)
            {
                Id = 1000,
                Ignored = false,
                CreateDate = new DateTime(2022, 1, 23)
            };

            // act
            var result = Mapper.Map<ClientError>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Ignored, Is.EqualTo(input.Ignored));
                Assert.That(result.Inserted, Is.EqualTo(input.CreateDate));
                Assert.That(result.MostCommonReferrer, Is.EqualTo(input.MostCommonReferrer));
                Assert.That(result.Url, Is.EqualTo(input.Url));
            });
        }

        [TestCase(TestName = "Map UrlTrackerClientErrorCollection to ClientErrorCollection")]
        public void Map_UrlTrackerClientErrorCollection_ClientErrorCollection()
        {
            // arrange
            var input = Core.Database.Entities.ClientErrorEntityCollection.Create(new[] { new ClientErrorEntity("http://example.com", false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound, default, default, default) }, 3);

            // act
            var result = Mapper.Map<Core.Models.ClientErrorCollection>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(input.Total));
                Assert.That(result.Count(), Is.EqualTo(input.Count()));
                Assert.That(result, Has.No.Null);
            });
        }
    }
}
