using System;
using System.Collections.Generic;
using System.Net;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Controllers.Models;
using UrlTracker.Web.Map;

namespace UrlTracker.Web.Tests.Map
{
    public class RequestMapTests : TestBase
    {
        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                new RequestMap(UmbracoContextFactoryAbstractionMock.UmbracoContextFactory)
            };
        }

        public override void SetUp()
        {
            UmbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetContentById(It.IsAny<int>())).Returns((int id) => new TestPublishedContent { Id = id });
        }

        [TestCase(TestName = "Map UpdateRedirectRequest to Redirect with content")]
        public void Map_UpdateRedirectRequest_Redirect_Content()
        {
            // arrange
            var input = new UpdateRedirectRequest
            {
                Culture = "nl-nl",
                ForceRedirect = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 24),
                Is404 = false,
                Notes = "Lorem ipsum",
                Occurrences = null,
                OldRegex = "dolor sit",
                OldUrl = "http://example.com",
                RedirectHttpCode = 302,
                RedirectNodeId = 1001,
                RedirectPassThroughQueryString = true,
                RedirectRootNodeId = 1002,
                RedirectUrl = "http://example.com/lorem",
                Referrer = null,
                Remove404 = false,
            };

            // act
            var result = Mapper.Map<Redirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.ForceRedirect));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.PassThroughQueryString, Is.EqualTo(input.RedirectPassThroughQueryString));
                Assert.That(result.SourceRegex, Is.EqualTo(input.OldRegex));
                Assert.That(result.SourceUrl, Is.EqualTo("http://example.com"));
                Assert.That(result.TargetNode, Is.Not.Null);
                Assert.That(result.TargetRootNode, Is.Not.Null);
                Assert.That(result.TargetStatusCode, Is.EqualTo(HttpStatusCode.Redirect));
                Assert.That(result.TargetUrl, Is.EqualTo(input.RedirectUrl));
            });
        }

        [TestCase(TestName = "Map UpdateRedirectRequest to Redirect without content")]
        public void Map_UpdateRedirectRequest_Redirect_NoContent()
        {
            // arrange
            var input = new UpdateRedirectRequest
            {
                RedirectRootNodeId = 1001
            };

            // act
            var result = Mapper.Map<Redirect>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.TargetNode, Is.Null);
                Assert.That(result.SourceUrl, Is.Null);
            });
        }
    }
}
