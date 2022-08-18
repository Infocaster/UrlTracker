using System.Web.Http.Results;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Web.Tests.Controllers
{
    public partial class UrlTrackerManagerControllerTests
    {
        [TestCase(TestName = "GetNodesWithDomains")]
        public void GetNodesWithDomains_Request_ReturnsUniqueNodes()
        {
            // arrange
            DomainProviderMock.Setup(obj => obj.GetDomains()).Returns(DomainCollection.Create(new[]
            {
                new Domain(1, 1, "https://example.com", "nl-NL", Url.Parse("https://example.com")),
                new Domain(2, 2, "https://example.com", "nl-NL", Url.Parse("https://example.com")),
                new Domain(3, 3, "https://example.com", "nl-NL", Url.Parse("https://example.com")),
            }));
            UmbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetContentAtRoot()).Returns(new[]
            {
                TestPublishedContent.Create(3),
                TestPublishedContent.Create(4)
            });

            // act
            var result = _testSubject.GetNodesWithDomains();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<System.Collections.Generic.IEnumerable<int>>>());
                var resultObject = (result as OkNegotiatedContentResult<System.Collections.Generic.IEnumerable<int>>)?.Content;
                Assert.That(resultObject, Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
            });
        }
    }
}
