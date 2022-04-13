using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Events;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Tests.Events
{
    public class ProcessedEventSubscriberTests : TestBase
    {
        private UrlTrackerHandledNotificationHandler? _testSubject;

        public override void SetUp()
        {
            _testSubject = new UrlTrackerHandledNotificationHandler(ClientErrorService, ClientErrorFilterCollection, new ConsoleLogger<UrlTrackerHandledNotificationHandler>(), RequestAbstraction);
        }

        [TestCase(TestName = "HandleAsync aborts processing if response is not 404")]
        public async Task HandleAsync_Not404_AbortsProcessing()
        {
            // arrange
            HttpContextMock!.ResponseMock.Setup(obj => obj.StatusCode).Returns(200);

            // act
            await _testSubject!.HandleAsync(new UrlTrackerHandled(HttpContextMock.Context), CancellationToken.None);

            // assert
            ClientErrorServiceMock!.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync aborts processing if request does not pass the filters")]
        public async Task HandleAsync_NotCandidate_AbortsProcessing()
        {
            // arrange
            HttpContextMock!.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            ClientErrorFilterCollectionMock!.Setup(obj => obj.EvaluateCandidacyAsync(HttpContextMock.Context)).ReturnsAsync(false);

            // act
            await _testSubject!.HandleAsync(new UrlTrackerHandled(HttpContextMock.Context), CancellationToken.None);

            // assert
            ClientErrorServiceMock!.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync processes request if it passes the filters")]
        public async Task HandleAsync_Candidate_IsProcessed()
        {
            // arrange
            HttpContextMock = new HttpContextMock(new Uri("http://example.com"));
            HttpContextMock.ResponseMock.Setup(obj => obj.StatusCode).Returns(404);
            RequestAbstractionMock!.Setup(obj => obj.GetReferrer(HttpContextMock.Request)).Returns(new Uri("http://example.com/lorem"));
            ClientErrorFilterCollectionMock!.Setup(obj => obj.EvaluateCandidacyAsync(HttpContextMock.Context)).ReturnsAsync(true);
            ClientErrorServiceMock!.Setup(obj => obj.AddAsync(It.IsAny<NotFound>())).ReturnsAsync((NotFound notFound) => notFound).Verifiable();

            // act
            await _testSubject!.HandleAsync(new UrlTrackerHandled(HttpContextMock.Context), CancellationToken.None);

            // assert
            ClientErrorServiceMock.Verify();
            ClientErrorServiceMock.VerifyNoOtherCalls();
        }
    }
}
