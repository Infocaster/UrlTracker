using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class NoLongerExistsResponseInterceptHandlerTests : TestBase
    {
        private NoLongerExistsResponseInterceptHandler? _testSubject;

        public override void SetUp()
        {
            _testSubject = new NoLongerExistsResponseInterceptHandler(ResponseAbstraction, UmbracoContextFactoryAbstractionMock!.UmbracoContextFactory);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var notFound = new UrlTrackerShallowClientError(HttpStatusCode.Gone);
            yield return new TestCaseData(notFound, 404, 410).SetName("HandleAsync rewrites response to 410 if status code is 404");
            yield return new TestCaseData(notFound, 200, 200).SetName("HandleAsync does nothing if status code is 200");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task HandleAsync_NormalFlow_PerformsAction(UrlTrackerShallowClientError notFound, int initialStatusCode, int expectedStatusCode)
        {
            // arrange
            HttpContextMock!.ResponseMock.SetupProperty(obj => obj.StatusCode, initialStatusCode);
            UmbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetResponseCode()).Returns(initialStatusCode);
            if (expectedStatusCode != initialStatusCode)
            {
                ResponseAbstractionMock!.Setup(obj => obj.Clear(HttpContextMock.Response)).Verifiable();
            }
            var input = new InterceptBase<UrlTrackerShallowClientError>(notFound);

            // act
            await _testSubject!.HandleAsync(context => Task.CompletedTask, HttpContextMock.Context, input);

            // assert
            HttpContextMock.ResponseMock.Verify();
            ResponseAbstractionMock!.Verify();
            Assert.That(HttpContextMock.Response.StatusCode, Is.EqualTo(expectedStatusCode));
        }
    }
}
