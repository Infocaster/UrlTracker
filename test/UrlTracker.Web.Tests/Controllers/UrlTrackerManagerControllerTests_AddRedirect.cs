using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Models;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers
{
    public partial class UrlTrackerManagerControllerTests
    {
        [TestCase(TestName = "AddRedirect calls add on RedirectService and returns an empty result")]
        public async Task AddRedirect_NormalFlow_ReturnsEmptyStatusCodeResult()
        {
            // arrange
            var input = new AddRedirectRequest();
            RedirectServiceMock!.Setup(obj => obj.AddAsync(It.IsAny<Redirect>()))
                               .ReturnsAsync((Redirect redirect) => redirect)
                               .Verifiable();

            // act
            var result = await _testSubject!.AddRedirect(input);

            // assert
            RedirectServiceMock.Verify();
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<OkResult>());
            });
        }

        [TestCase(TestName = "AddRedirect removes related 404 entries if flag is set")]
        public async Task AddRedirect_Remove404_Removes404()
        {
            // arrange
            var input = new AddRedirectRequest
            {
                Remove404 = true,
                OldUrl = "http://example.com/lorem"
            };
            RedirectServiceMock.Setup(obj => obj.AddAsync(It.IsAny<Redirect>()))
                               .ReturnsAsync((Redirect redirect) => redirect)
                               .Verifiable();
            ClientErrorServiceMock.Setup(obj => obj.GetAsync(It.IsAny<string>())).ReturnsAsync((string s) => new ClientError(s));
            ClientErrorServiceMock.Setup(obj => obj.DeleteAsync(It.IsAny<ClientError>()))
                                  .Verifiable();
            RequestModelPatcherMock.Setup(obj => obj.Patch(It.IsAny<AddRedirectRequest>()))
                                   .Returns((AddRedirectRequest request) => request);

            // act
            var result = await _testSubject.AddRedirect(input);

            // assert
            RedirectServiceMock.Verify();
            ClientErrorServiceMock.Verify();
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<OkResult>());
            });
        }

        [TestCase(TestName = "AddRedirect returns BadRequest if flag is set but no client error could be found")]
        public async Task AddRedirect_ClientErrorNotFound_ReturnBadRequest()
        {
            // arrange
            var input = new AddRedirectRequest
            {
                Remove404 = true,
                OldUrl = "http://example.com/lorem"
            };
            RequestModelPatcherMock.Setup(obj => obj.Patch(It.IsAny<AddRedirectRequest>()))
                                   .Returns((AddRedirectRequest request) => request);
            ClientErrorServiceMock.Setup(obj => obj.GetAsync(It.IsAny<string>())).ReturnsAsync((string s) => null);

            // act
            var result = await _testSubject.AddRedirect(input);

            // assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
