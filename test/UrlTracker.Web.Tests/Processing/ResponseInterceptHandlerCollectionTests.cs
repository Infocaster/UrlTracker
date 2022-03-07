using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class ResponseInterceptHandlerCollectionTests : TestBase
    {
        private ResponseInterceptHandlerCollection _testSubject;

        public override void SetUp()
        {
            _testSubject = new ResponseInterceptHandlerCollection(() => new List<IResponseInterceptHandler> { ResponseInterceptHandler });
        }

        [TestCase(true, TestName = "Get returns handler if it can handle the intercept")]
        [TestCase(false, TestName = "Get returns null if no handler can handle the intercept")]
        public void Get_NormalFlow_PassesInterceptToHandler(bool canHandle)
        {
            // arrange
            var input = Mock.Of<IIntercept>();
            ResponseInterceptHandlerMock.Setup(obj => obj.CanHandle(input))
                .Returns(canHandle)
                .Verifiable();

            // act
            var result = _testSubject.Get(input);

            // assert
            ResponseInterceptHandlerMock.Verify();
            ResponseInterceptHandlerMock.VerifyNoOtherCalls();
            Assert.That(result is null, Is.Not.EqualTo(canHandle));
        }
    }
}
