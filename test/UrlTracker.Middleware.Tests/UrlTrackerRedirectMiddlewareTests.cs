﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;

namespace UrlTracker.Middleware.Tests
{
    public class UrlTrackerRedirectMiddlewareTests : TestBase
    {
        private UrlTrackerRedirectMiddleware? _testSubject;

        protected override HttpContextMock CreateHttpContextMock()
            => new(new Uri("http://example.com/lorem"));

        public override void SetUp()
        {
            _testSubject = new UrlTrackerRedirectMiddleware(context => Task.CompletedTask, new VoidLogger<UrlTrackerRedirectMiddleware>(), InterceptService, ResponseInterceptHandlerCollection, RequestInterceptFilterCollection, RuntimeState);
        }

        [TestCase(TestName = "HandleAsync processes request")]
        public async Task HandleAsync_NormalFlow_ProcessesRequest()
        {
            // arrange
            InterceptBase<object> intercept = new(new object());
            RequestInterceptFilterCollectionMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(true);
            RuntimeStateMock!.Setup(obj => obj.Level).Returns(RuntimeLevel.Run);
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

        [TestCase(TestName = "HandleAsync cuts intercept short if the incoming request is not a candidate")]
        public async Task HandleAsync_NoValidCandidate_InterceptCutShort()
        {
            // arrange
            RequestInterceptFilterCollectionMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>())).ReturnsAsync(false);
            RuntimeStateMock!.Setup(obj => obj.Level).Returns(RuntimeLevel.Run);

            // act
            await _testSubject!.InvokeAsync(HttpContextMock!.Context);

            // assert
            InterceptServiceMock!.VerifyNoOtherCalls();
        }

        [TestCase(TestName = "HandleAsync cuts intercept short if umbraco hasn't completely initialised yet")]
        public async Task HandleAsync_UmbracoNotInitialised_InterceptCutShort()
        {
            // arrange
            RuntimeStateMock!.Setup(obj => obj.Level).Returns(RuntimeLevel.Install);

            // act
            await _testSubject!.InvokeAsync(HttpContextMock!.Context);

            // assert
            RequestInterceptFilterCollectionMock!.VerifyNoOtherCalls();
            InterceptServiceMock!.VerifyNoOtherCalls();
        }
    }
}
