using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Processing;

namespace UrlTracker.Middleware.Tests
{
    public class UrlTrackerRedirectMiddlewareTests
    {
        private HttpContextMock _httpContextMock;
        private Mock<IInterceptService> _interceptServiceMock;
        private Mock<IResponseInterceptHandlerCollection> _responseInterceptHandlerCollectionMock;
        private Mock<IRequestInterceptFilterCollection> _requestInterceptFilterCollectionMock;
        private Mock<IRuntimeState> _runtimeStateMock;
        private Mock<IResponseInterceptHandler> _responseInterceptHandlerMock;
        private UrlTrackerRedirectMiddleware? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _httpContextMock = new HttpContextMock(new Uri("http://example.com/lorem"));
            _interceptServiceMock = new Mock<IInterceptService>();
            _responseInterceptHandlerCollectionMock = new Mock<IResponseInterceptHandlerCollection>();
            _requestInterceptFilterCollectionMock = new Mock<IRequestInterceptFilterCollection>();
            _runtimeStateMock = new Mock<IRuntimeState>();
            _responseInterceptHandlerMock = new Mock<IResponseInterceptHandler>();
            _testSubject = new UrlTrackerRedirectMiddleware(
                context => Task.CompletedTask,
                new VoidLogger<UrlTrackerRedirectMiddleware>(),
                _interceptServiceMock.Object,
                _responseInterceptHandlerCollectionMock.Object,
                _requestInterceptFilterCollectionMock.Object,
                _runtimeStateMock.Object);
        }

        [TestCase(TestName = "HandleAsync processes request")]
        public async Task HandleAsync_NormalFlow_ProcessesRequest()
        {
            // arrange
            InterceptBase<object> intercept = new(new object());
            _requestInterceptFilterCollectionMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(true);
            _runtimeStateMock!.Setup(obj => obj.Level).Returns(RuntimeLevel.Run);
            _interceptServiceMock!.Setup(obj => obj.GetAsync(It.IsAny<Url>()))
                                .ReturnsAsync(intercept)
                                .Verifiable();
            _responseInterceptHandlerCollectionMock!.Setup(obj => obj.Get(intercept))
                                                  .Returns(_responseInterceptHandlerMock.Object)
                                                  .Verifiable();
            _responseInterceptHandlerMock!.Setup(obj => obj.HandleAsync(It.IsAny<RequestDelegate>(), _httpContextMock.Context, intercept))
                                        .Verifiable();

            // act
            await _testSubject!.InvokeAsync(_httpContextMock.Context);

            // assert
            _interceptServiceMock.Verify();
            _responseInterceptHandlerCollectionMock.Verify();
            _responseInterceptHandlerMock.Verify();
        }

        [TestCase(TestName = "HandleAsync cuts intercept short if the incoming request is not a candidate")]
        public async Task HandleAsync_NoValidCandidate_InterceptCutShort()
        {
            // arrange
            _requestInterceptFilterCollectionMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(false);
            _runtimeStateMock!.Setup(obj => obj.Level).Returns(RuntimeLevel.Run);

            // act
            await _testSubject!.InvokeAsync(_httpContextMock.Context);

            // assert
            _interceptServiceMock!.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync cuts intercept short if umbraco hasn't completely initialised yet")]
        public async Task HandleAsync_UmbracoNotInitialised_InterceptCutShort()
        {
            // arrange
            _runtimeStateMock!.Setup(obj => obj.Level).Returns(RuntimeLevel.Install);

            // act
            await _testSubject!.InvokeAsync(_httpContextMock.Context);

            // assert
            _requestInterceptFilterCollectionMock!.VerifyNoOtherCalls();
            _interceptServiceMock!.VerifyNoOtherCalls();
        }
    }
}
