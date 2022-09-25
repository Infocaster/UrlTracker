using System.Net;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Backoffice.UI.Map;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Backoffice.UI.Tests.Map
{
    public class CsvMapTests : TestBase
    {
        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                new CsvMap(UmbracoContextFactoryAbstractionMock!.UmbracoContextFactory)
            };
        }

        public override void SetUp()
        {
            UmbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetContentById(It.IsAny<int>())).Returns((int id) => TestPublishedContent.Create(id));
        }

        [TestCase(TestName = "Map Redirect to CsvRedirect with content")]
        public void Map_Redirect_CsvRedirect_content()
        {
            // arrange
            var input = new Redirect
            {
                Culture = "nl-nl",
                Force = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 24),
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
            var result = Mapper!.Map<CsvRedirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.PassThroughQueryString, Is.EqualTo(input.RetainQuery));
                Assert.That(result.SourceRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.SourceUrl, Is.EqualTo(input.SourceUrl));
                Assert.That(result.TargetNodeId, Is.EqualTo(input.TargetNode.Id));
                Assert.That(result.TargetRootNodeId, Is.EqualTo(input.TargetRootNode.Id));
                Assert.That(result.TargetStatusCode, Is.EqualTo(302));
                Assert.That(result.TargetUrl, Is.EqualTo(input.TargetUrl));
            });
        }

        [TestCase(TestName = "Map Redirect to CsvRedirect without content")]
        public void Map_Redirect_CsvRedirect_NoContent()
        {
            // arrange
            var input = new Redirect();

            // act
            var result = Mapper!.Map<CsvRedirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SourceUrl, Is.Null);
                Assert.That(result.TargetNodeId, Is.Null);
                Assert.That(result.TargetRootNodeId, Is.Null);
            });
        }

        [TestCase(TestName = "Map CsvRedirect to Redirect with content")]
        public void Map_CsvRedirect_Redirect_Content()
        {
            // arrange
            var input = new CsvRedirect
            {
                Culture = "nl-nl",
                Force = true,
                Notes = "lorem ipsum",
                PassThroughQueryString = true,
                SourceRegex = "dolor sit",
                SourceUrl = "http://example.com",
                TargetNodeId = 1000,
                TargetRootNodeId = 1001,
                TargetStatusCode = 302,
                TargetUrl = "http://example.com/lorem"
            };

            // act
            var result = Mapper!.Map<Redirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.Null);
                Assert.That(result.Inserted, Is.EqualTo(default(DateTime)));
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.RetainQuery, Is.EqualTo(input.PassThroughQueryString));
                Assert.That(result.SourceRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.SourceUrl, Is.EqualTo("http://example.com"));
                Assert.That(result.TargetNode, Is.Not.Null);
                Assert.That(result.TargetRootNode, Is.Not.Null);
                Assert.That(result.TargetStatusCode, Is.EqualTo(HttpStatusCode.Redirect));
                Assert.That(result.TargetUrl, Is.EqualTo(input.TargetUrl));
            });
        }

        [TestCase(TestName = "Map CsvRedirect to Redirect without content")]
        public void Map_CsvRedirect_Redirect_NoContent()
        {
            // arrange
            var input = new CsvRedirect();

            // act
            var result = Mapper!.Map<Redirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SourceUrl, Is.Null);
                Assert.That(result.TargetNode, Is.Null);
                Assert.That(result.TargetRootNode, Is.Null);
            });
        }
    }
}
