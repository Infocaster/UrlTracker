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
                Force = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 24),
                RetainQuery = true,
            };

            // act
            var result = Mapper!.Map<CsvRedirect>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.PassThroughQueryString, Is.EqualTo(input.RetainQuery));
                Assert.That(result.TargetStatusCode, Is.EqualTo(302));
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
                Assert.That(result.Force, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.Null);
                Assert.That(result.Inserted, Is.EqualTo(default(DateTime)));
                Assert.That(result.RetainQuery, Is.EqualTo(input.PassThroughQueryString));
            });
        }
    }
}
