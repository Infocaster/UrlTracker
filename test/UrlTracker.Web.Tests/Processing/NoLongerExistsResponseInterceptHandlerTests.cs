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
        private NoLongerExistsResponseInterceptHandler _testSubject;

        public override void SetUp()
        {
            _testSubject = new NoLongerExistsResponseInterceptHandler(CompleteRequestAbstraction);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var notFound = new UrlTrackerShallowClientError
            {
                TargetStatusCode = HttpStatusCode.Gone
            };
            yield return new TestCaseData(notFound, 404, 410);
            yield return new TestCaseData(notFound, 200, 200);
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task HandleAsync_NormalFlow_PerformsAction(UrlTrackerShallowClientError notFound, int initialStatusCode, int expectedStatusCode)
        {
            // arrange
            HttpContextMock.ResponseMock.SetupProperty(obj => obj.StatusCode, initialStatusCode);
            if (expectedStatusCode != initialStatusCode)
            {
                HttpContextMock.ResponseMock.Setup(obj => obj.Clear()).Verifiable();
                CompleteRequestAbstractionMock.Setup(obj => obj.CompleteRequest(HttpContextMock.Context)).Verifiable();
            }
            var input = new InterceptBase<UrlTrackerShallowClientError>(notFound);

            // act
            await _testSubject.HandleAsync(HttpContextMock.Context, input);

            // assert
            HttpContextMock.ResponseMock.Verify();
            CompleteRequestAbstractionMock.Verify();
            Assert.That(HttpContextMock.Response.StatusCode, Is.EqualTo(expectedStatusCode));
        }
    }
}
