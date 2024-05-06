using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core.Models;
using UrlTracker.Middleware.Background;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Processing;

namespace UrlTracker.Middleware.Tests
{
    public class UrlTrackerClientErrorTrackingMiddlewareTests
    {
        private Mock<IClientErrorFilterCollection> _clientErrorFilterCollectionMock;
        private Mock<IRequestAbstraction> _requestAbstractionMock;
        private Mock<IClientErrorProcessorQueue> _clientErrorProcessorQueueMock;
        private UrlTrackerClientErrorTrackingMiddleware _testSubject = null!;
        private Mock<IRuntimeState> _runtimeStateMock;
        private Mock<IOptionsMonitor<RequestHandlerSettings>> _requestHandlerSettingsMock;

        [SetUp]
        public void SetUp()
        {
            _runtimeStateMock = new Mock<IRuntimeState>();
            _runtimeStateMock.SetupGet(obj => obj.Level).Returns(RuntimeLevel.Run);
            _requestHandlerSettingsMock = new Mock<IOptionsMonitor<RequestHandlerSettings>>();
            _requestHandlerSettingsMock.Setup(obj => obj.CurrentValue).Returns(new RequestHandlerSettings { AddTrailingSlash = false });
            _clientErrorFilterCollectionMock = new Mock<IClientErrorFilterCollection>();
            _requestAbstractionMock = new Mock<IRequestAbstraction>();
            _clientErrorProcessorQueueMock = new Mock<IClientErrorProcessorQueue>();
            _testSubject = new UrlTrackerClientErrorTrackingMiddleware(
                context => Task.CompletedTask,
                _clientErrorFilterCollectionMock.Object,
                new VoidLogger<UrlTrackerClientErrorTrackingMiddleware>(),
                _requestHandlerSettingsMock.Object,
                _requestAbstractionMock.Object,
                _runtimeStateMock.Object,
                _clientErrorProcessorQueueMock.Object);
        }

        [TestCase(TestName = "HandleAsync aborts processing if Umbraco is not completely initialised")]
        public async Task HandleAsync_RuntimeStateLessThanRun_AbortsProcessing()
        {
            // arrange
            var httpContextMock = new HttpContextMock();
            httpContextMock.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            httpContextMock.SetupUrl(Url.Parse("http://example.com/"));
            _clientErrorFilterCollectionMock.Setup(obj => obj.EvaluateCandidacyAsync(It.Is<HttpContext>(h => h == httpContextMock.Context))).ReturnsAsync(true);
            _runtimeStateMock.SetupGet(obj => obj.Level).Returns(RuntimeLevel.Install);

            // act
            await _testSubject.InvokeAsync(httpContextMock.Context);

            // assert
            _clientErrorProcessorQueueMock.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync aborts processing if request does not pass the filters")]
        public async Task HandleAsync_NotCandidate_AbortsProcessing()
        {
            // arrange
            var httpContextMock = new HttpContextMock();
            httpContextMock.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            _clientErrorFilterCollectionMock.Setup(obj => obj.EvaluateCandidacyAsync(It.Is<HttpContext>(h => h == httpContextMock.Context))).ReturnsAsync(false);
            httpContextMock.SetupUrl(Url.Parse("http://example.com/"));

            // act
            await _testSubject.InvokeAsync(httpContextMock.Context);

            // assert
            _clientErrorProcessorQueueMock!.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync processes request if it passes the filters")]
        public async Task HandleAsync_Candidate_IsProcessed()
        {
            // arrange
            var httpContextMock = new HttpContextMock();
            httpContextMock = new HttpContextMock(new Uri("http://example.com"));
            httpContextMock.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            _requestAbstractionMock.Setup(obj => obj.GetReferrer(httpContextMock.Request)).Returns(new Uri("http://example.com/lorem"));
            _clientErrorFilterCollectionMock.Setup(obj => obj.EvaluateCandidacyAsync(It.Is<HttpContext>(h => h == httpContextMock.Context))).ReturnsAsync(true);
            _clientErrorProcessorQueueMock.Setup(obj => obj.WriteAsync(It.Is<ClientErrorProcessorItem>(i => i.Url == "http://example.com" && i.Referrer == "http://example.com/lorem"))).Verifiable();

            // act
            await _testSubject.InvokeAsync(httpContextMock.Context);

            // assert
            _clientErrorProcessorQueueMock.Verify();
            _clientErrorProcessorQueueMock.VerifyNoOtherCalls();
        }
    }
}
