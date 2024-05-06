using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Processing.Handling;

namespace UrlTracker.Web.Tests.Processing
{
    public class NoLongerExistsResponseInterceptHandlerTests
    {
        private Mock<IResponseAbstraction> _responseAbstractionMock;
        private UmbracoContextFactoryAbstractionMock _umbracoContextFactoryAbstractionMock;
        private NoLongerExistsResponseInterceptHandler? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _responseAbstractionMock = new Mock<IResponseAbstraction>();
            _umbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            _testSubject = new NoLongerExistsResponseInterceptHandler(_responseAbstractionMock.Object, _umbracoContextFactoryAbstractionMock!.UmbracoContextFactory);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var clientError = new ClientErrorEntity("https://example.com", false, Core.Defaults.DatabaseSchema.ClientErrorStrategies.NoLongerExists);
            yield return new TestCaseData(clientError, 404, 410).SetName("HandleAsync rewrites response to 410 if status code is 404");
            yield return new TestCaseData(clientError, 200, 200).SetName("HandleAsync does nothing if status code is 200");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task HandleAsync_NormalFlow_PerformsAction(IClientError clientError, int initialStatusCode, int expectedStatusCode)
        {
            // arrange
            var httpContextMock = new HttpContextMock();
            httpContextMock!.ResponseMock.SetupProperty(obj => obj.StatusCode, initialStatusCode);
            _umbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetResponseCode()).Returns(initialStatusCode);
            if (expectedStatusCode != initialStatusCode)
            {
                _responseAbstractionMock!.Setup(obj => obj.Clear(httpContextMock.Response)).Verifiable();
            }
            var input = new InterceptBase<IClientError>(clientError);

            // act
            await _testSubject!.HandleAsync(context => Task.CompletedTask, httpContextMock.Context, input);

            // assert
            httpContextMock.ResponseMock.Verify();
            _responseAbstractionMock!.Verify();
            Assert.That(httpContextMock.Response.StatusCode, Is.EqualTo(expectedStatusCode));
        }
    }
}
