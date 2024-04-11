using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Middleware.Background;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;

namespace UrlTracker.Middleware.Tests
{
    public class UrlTrackerClientErrorTrackingMiddlewareTests : TestBase
    {
        private UrlTrackerClientErrorTrackingMiddleware _testSubject = null!;

        public override void SetUp()
        {
            RuntimeStateMock.SetupGet(obj => obj.Level).Returns(RuntimeLevel.Run);
            RequestHandlerSettingsMock.Setup(obj => obj.CurrentValue).Returns(new RequestHandlerSettings { AddTrailingSlash = false });
            _testSubject = new UrlTrackerClientErrorTrackingMiddleware(context => Task.CompletedTask, ClientErrorFilterCollection, new VoidLogger<UrlTrackerClientErrorTrackingMiddleware>(), RequestHandlerSettings, RequestAbstraction, RuntimeState, ClientErrorProcessorQueue);
        }

        [TestCase(TestName = "HandleAsync aborts processing if Umbraco is not completely initialised")]
        public async Task HandleAsync_RuntimeStateLessThanRun_AbortsProcessing()
        {
            // arrange
            HttpContextMock.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            HttpContextMock.SetupUrl(Url.Parse("http://example.com/"));
            ClientErrorFilterCollectionMock.Setup(obj => obj.EvaluateCandidacyAsync(It.Is<HttpContext>(h => h == HttpContextMock.Context))).ReturnsAsync(true);
            RuntimeStateMock.SetupGet(obj => obj.Level).Returns(RuntimeLevel.Install);

            // act
            await _testSubject.InvokeAsync(HttpContextMock.Context);

            // assert
            ClientErrorProcessorQueueMock.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync aborts processing if request does not pass the filters")]
        public async Task HandleAsync_NotCandidate_AbortsProcessing()
        {
            // arrange
            HttpContextMock.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            ClientErrorFilterCollectionMock.Setup(obj => obj.EvaluateCandidacyAsync(It.Is<HttpContext>(h => h == HttpContextMock!.Context))).ReturnsAsync(false);
            HttpContextMock.SetupUrl(Url.Parse("http://example.com/"));

            // act
            await _testSubject.InvokeAsync(HttpContextMock.Context);

            // assert
            ClientErrorProcessorQueueMock!.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync processes request if it passes the filters")]
        public async Task HandleAsync_Candidate_IsProcessed()
        {
            // arrange
            HttpContextMock = new HttpContextMock(new Uri("http://example.com"));
            HttpContextMock.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            RequestAbstractionMock.Setup(obj => obj.GetReferrer(HttpContextMock.Request)).Returns(new Uri("http://example.com/lorem"));
            ClientErrorFilterCollectionMock.Setup(obj => obj.EvaluateCandidacyAsync(It.Is<HttpContext>(h => h == HttpContextMock.Context))).ReturnsAsync(true);
            ClientErrorProcessorQueueMock.Setup(obj => obj.WriteAsync(It.Is<ClientErrorProcessorItem>(i => i.Url == "http://example.com" && i.Referrer == "http://example.com/lorem"))).Verifiable();

            // act
            await _testSubject.InvokeAsync(HttpContextMock.Context);

            // assert
            ClientErrorProcessorQueueMock.Verify();
            ClientErrorProcessorQueueMock.VerifyNoOtherCalls();
        }
    }
}
