using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Events;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;

namespace UrlTracker.Web.Tests.Events
{
    public class UrlTrackerMiddlewareTests : TestBase
    {
        private UrlTrackerMiddleware? _testSubject;

        protected override HttpContextMock CreateHttpContextMock()
            => new(new Uri("http://example.com/lorem"));

        public override void SetUp()
        {
            _testSubject = new UrlTrackerMiddleware(context => Task.CompletedTask, new ConsoleLogger<UrlTrackerMiddleware>(), InterceptService, ResponseInterceptHandlerCollection, RequestInterceptFilterCollection, Mock.Of<IEventAggregator>());
        }

        [TestCase(TestName = "HandleAsync processes request")]
        public async Task HandleAsync_NormalFlow_ProcessesRequest()
        {
            // arrange
            InterceptBase<object> intercept = new(new object());
            RequestInterceptFilterCollectionMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(true);
            InterceptServiceMock!.Setup(obj => obj.GetAsync(It.IsAny<Url>()))
                                .ReturnsAsync(intercept)
                                .Verifiable();
            ResponseInterceptHandlerCollectionMock!.Setup(obj => obj.Get(intercept))
                                                  .Returns(ResponseInterceptHandler)
                                                  .Verifiable();
            ResponseInterceptHandlerMock!.Setup(obj => obj.HandleAsync(It.IsAny<RequestDelegate>(), HttpContextMock!.Context, intercept))
                                        .Verifiable();

            // act
            await _testSubject!.InvokeAsync(HttpContextMock!.Context);

            // assert
            InterceptServiceMock.Verify();
            ResponseInterceptHandlerCollectionMock.Verify();
            ResponseInterceptHandlerMock.Verify();
        }

        [TestCase(TestName = "HandleAsync cuts the intercept short if no intercept was found")]
        public async Task HandleAsync_NoIntercept_InterceptCutShort()
        {
            // arrange
            RequestInterceptFilterCollectionMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(true);
            InterceptServiceMock!.Setup(obj => obj.GetAsync(It.IsAny<Url>()))
                .ReturnsAsync(null as IIntercept)
                .Verifiable();

            // act
            await _testSubject!.InvokeAsync(HttpContextMock!.Context);

            // assert
            InterceptServiceMock.Verify();
            ResponseInterceptHandlerCollectionMock!.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync cuts intercept short if the incoming request is not a candidate")]
        public async Task HandleAsync_NoValidCandidate_InterceptCutShort()
        {
            // arrange
            RequestInterceptFilterCollectionMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(false);

            // act
            await _testSubject!.InvokeAsync(HttpContextMock!.Context);

            // assert
            InterceptServiceMock!.VerifyNoOtherCalls();
        }
    }
}
