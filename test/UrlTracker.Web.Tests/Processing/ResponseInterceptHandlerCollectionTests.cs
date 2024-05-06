using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Web.Processing;
using UrlTracker.Web.Processing.Handling;

namespace UrlTracker.Web.Tests.Processing
{
    public class ResponseInterceptHandlerCollectionTests
    {
        private Mock<ISpecificResponseInterceptHandler> _responseInterceptHandlerMock;
        private ResponseInterceptHandlerCollection? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _responseInterceptHandlerMock = new Mock<ISpecificResponseInterceptHandler>();
            _testSubject = new ResponseInterceptHandlerCollection(() => new List<ISpecificResponseInterceptHandler> { _responseInterceptHandlerMock.Object }, new LastChanceResponseInterceptHandler(new VoidLogger<LastChanceResponseInterceptHandler>()));
        }

        [TestCase(true, TestName = "Get returns handler if it can handle the intercept")]
        [TestCase(false, TestName = "Get returns last chance handler if no handler can handle the intercept")]
        public void Get_NormalFlow_PassesInterceptToHandler(bool canHandle)
        {
            // arrange
            var input = Mock.Of<IIntercept>();
            _responseInterceptHandlerMock!.Setup(obj => obj.CanHandle(input))
                .Returns(canHandle)
                .Verifiable();

            // act
            var result = _testSubject!.Get(input);

            // assert
            _responseInterceptHandlerMock.Verify();
            _responseInterceptHandlerMock.VerifyNoOtherCalls();
            Assert.That(result is LastChanceResponseInterceptHandler, Is.Not.EqualTo(canHandle));
        }
    }
}
