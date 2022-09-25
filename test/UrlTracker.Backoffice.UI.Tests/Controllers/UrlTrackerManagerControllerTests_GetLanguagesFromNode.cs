using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Web.Tests.Controllers
{
    public partial class UrlTrackerManagerControllerTests
    {
        [TestCase(TestName = "GetLanguagesOutNodeDomains returns unique languages from a given node")]
        public void GetLanguagesOutNodeDomains_NormalFlow_ReturnsLanguages()
        {
            // arrange
            var domain1 = new Domain(1, 1000, "example.com", "en-US", Url.Parse("example.com"));
            var domain2 = new Domain(2, 1000, "nl.example.com", "en-US", Url.Parse("nl.example.com"));
            DomainProviderMock!.Setup(obj => obj.GetDomains(It.IsAny<int>())).Returns(DomainCollection.Create(new[]
            {
                domain1,
                domain2
            }));

            // act
            var result = _testSubject!.GetLanguagesOutNodeDomains(new GetLanguagesFromNodeRequest { NodeId = 1000 });

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<OkObjectResult>());
                var contentResult = result as OkObjectResult;
                Assert.That((contentResult?.Value as List<GetLanguagesFromNodeResponseLanguage>)?.Count, Is.EqualTo(1));
            });
        }
    }
}
