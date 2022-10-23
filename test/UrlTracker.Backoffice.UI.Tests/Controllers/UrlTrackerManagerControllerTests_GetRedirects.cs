using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.UI.Tests.Controllers
{
    public partial class UrlTrackerManagerControllerTests
    {
        [TestCase(TestName = "GetRedirects makes correct call to RedirectService and returns result")]
        public async Task GetRedirects_NormalFlow_ReturnsListOfRedirects()
        {
            // arrange
            var input = new GetRedirectsRequest
            {
                SortType = OrderBy.OccurrencesAsc,
                Amount = 1,
                Skip = 0
            };
            RedirectServiceMock!.Setup(obj => obj.GetAsync(It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<string>(), Core.Database.Models.OrderBy.Occurrences, false))
                                .ReturnsAsync(RedirectCollection.Create(Enumerable.Empty<Redirect>()))
                                .Verifiable();

            // act
            var result = await _testSubject!.GetRedirects(input);

            // assert
            RedirectServiceMock.Verify();
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}
