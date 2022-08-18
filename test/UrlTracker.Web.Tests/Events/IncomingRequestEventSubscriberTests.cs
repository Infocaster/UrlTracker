using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Events;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Tests.Events
{
    public class IncomingRequestEventSubscriberTests : TestBase
    {
        private IncomingRequestEventSubscriber _testSubject;

        protected override HttpContextMock CreateHttpContextMock()
            => new HttpContextMock(new Uri("http://example.com/lorem"));

        public override void SetUp()
        {
            _testSubject = new IncomingRequestEventSubscriber(new VoidLogger(), InterceptService, ResponseInterceptHandlerCollection, RequestInterceptFilterCollection, Mock.Of<IEventPublisher<ProcessedEventArgs>>());
        }

        [TestCase(TestName = "HandleAsync processes request")]
        public async Task HandleAsync_NormalFlow_ProcessesRequest()
        {
            // arrange
            InterceptBase<object> intercept = new InterceptBase<object>(new object());
            RequestInterceptFilterCollectionMock.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(true);
            InterceptServiceMock.Setup(obj => obj.GetAsync(It.IsAny<Url>()))
                                .ReturnsAsync(intercept)
                                .Verifiable();
            ResponseInterceptHandlerCollectionMock.Setup(obj => obj.Get(intercept))
                                                  .Returns(ResponseInterceptHandler)
                                                  .Verifiable();
            ResponseInterceptHandlerMock.Setup(obj => obj.HandleAsync(HttpContextMock.Context, intercept))
                                        .Verifiable();

            // act
            await _testSubject.HandleAsync(new object(), new IncomingRequestEventArgs(HttpContextMock.Context));

            // assert
            InterceptServiceMock.Verify();
            ResponseInterceptHandlerCollectionMock.Verify();
            ResponseInterceptHandlerMock.Verify();
        }

        [TestCase(TestName = "HandleAsync cuts intercept short if the incoming request is not a candidate")]
        public async Task HandleAsync_NoValidCandidate_InterceptCutShort()
        {
            // arrange
            RequestInterceptFilterCollectionMock.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(false);

            // act
            await _testSubject.HandleAsync(new object(), new IncomingRequestEventArgs(HttpContextMock.Context));

            // assert
            InterceptServiceMock.VerifyNoOtherCalls();
        }
    }
}
