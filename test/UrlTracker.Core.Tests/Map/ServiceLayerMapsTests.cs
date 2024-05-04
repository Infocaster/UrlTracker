using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Map
{
    public class ServiceLayerMapsTests
    {
        private UmbracoMapper _mapper;
        private Mock<IStrategyMapCollection> _strategyMapCollectionMock;
        private UmbracoContextFactoryAbstractionMock _umbracoContextFactoryAbstractionMock;

        private ICollection<IMapDefinition> CreateMappers()
        {
            return new[]
            {
                new ServiceLayerMaps(_strategyMapCollectionMock.Object)
            };
        }

        [SetUp]
        public void SetUp()
        {
            _strategyMapCollectionMock = new Mock<IStrategyMapCollection>();
            _strategyMapCollectionMock.Setup(obj => obj.Map<ISourceStrategy>(It.IsAny<EntityStrategy>())).Returns((EntityStrategy es) => new UrlSourceStrategy(es.Value));
            _strategyMapCollectionMock.Setup(obj => obj.Map<ITargetStrategy>(It.IsAny<EntityStrategy>())).Returns(new ContentPageTargetStrategy(TestPublishedContent.Create(1234), "en-US"));
            _umbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            _umbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetContentById(It.IsAny<int>())).Returns((int id) => TestPublishedContent.Create(id));
            _mapper = new UmbracoMapper(new MapDefinitionCollection(CreateMappers), Mock.Of<ICoreScopeProvider>(), new VoidLogger<UmbracoMapper>());
        }

        [TestCase(TestName = "Map IRedirect to Redirect")]
        public void Map_UrlTrackerRedirect_Redirect()
        {
            // arrange
            IRedirect input = new RedirectEntity(default, default, default, EntityStrategy.UrlSource("https://example.com"), EntityStrategy.UrlTarget("https://example.com"))
            {
                CreateDate = new DateTime(2022, 1, 23),
                Force = true,
                Id = 1000,
                RetainQuery = true,
                Permanent = false,
            };

            // act
            var result = _mapper!.Map<Redirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.RetainQuery, Is.EqualTo(input.RetainQuery));
                Assert.That(result.Inserted, Is.EqualTo(input.CreateDate));
                Assert.That(result.Permanent, Is.EqualTo(input.Permanent));
            });
        }

        [TestCase(TestName = "Map UrlTrackerRedirectCollection to RedirectCollection")]
        public void Map_UrlTrackerRedirectCollection_RedirectCollection()
        {
            // arrange
            var input = Database.Entities.RedirectEntityCollection.Create(new[] { new RedirectEntity(default, default, default, EntityStrategy.UrlSource("https://example.com"), EntityStrategy.UrlTarget("https://example.com")) }, 3);

            // act
            var result = _mapper!.Map<Core.Models.RedirectCollection>(input)!;

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
                Force = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 23),
                RetainQuery = true,
                Permanent = true
            };

            // act
            var result = _mapper!.Map<IRedirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.CreateDate, Is.EqualTo(input.Inserted));
                Assert.That(result.RetainQuery, Is.EqualTo(input.RetainQuery));
                Assert.That(result.Permanent, Is.EqualTo(input.Permanent));
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
            var result = _mapper!.Map<IClientError>(input)!;

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
            var input = new ClientErrorEntity("http://example.com/lorem", false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound)
            {
                Id = 1000,
                Ignored = false,
                CreateDate = new DateTime(2022, 1, 23)
            };

            // act
            var result = _mapper!.Map<ClientError>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Ignored, Is.EqualTo(input.Ignored));
                Assert.That(result.Inserted, Is.EqualTo(input.CreateDate));
                Assert.That(result.Url, Is.EqualTo(input.Url));
            });
        }
    }
}
