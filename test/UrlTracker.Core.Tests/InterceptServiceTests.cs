using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests
{
    public class InterceptServiceTests : TestBase
    {
        private InterceptService _testSubject;

        public override void SetUp()
        {
            _testSubject = new InterceptService(IntermediateInterceptService, InterceptConverterCollection);
        }

        [TestCase(TestName = "GetAsync with absolute url does not throw exceptions")]
        public void GetAsync_NormalFlow_ReturnsResult()
        {
            // arrange
            ICachableIntercept output = new CachableInterceptBase<object>(new object());
            IntermediateInterceptServiceMock.Setup(obj => obj.GetAsync(It.IsAny<Url>(), null)).ReturnsAsync(output);
            InterceptConverterMock.Setup(obj => obj.ConvertAsync(output)).ReturnsAsync((ICachableIntercept c) => c);

            // act
            Task result() => _testSubject.GetAsync(Url.Parse("http://example.com/lorem"));

            // assert
            Assert.DoesNotThrowAsync(result);
        }

        [TestCase(TestName = "GetAsync with relative url throws argument exception")]
        public void GetAsyncUrl_RelativeUrl_ThrowsException()
        {
            // arrange
            var input = Url.Parse("/lorem");

            // act
            Task result() => _testSubject.GetAsync(input);

            // assert
            Assert.Multiple(() =>
            {
                var ex = Assert.ThrowsAsync<ArgumentException>(result);
                Assert.That(ex.ParamName, Is.EqualTo("url"));
            });
        }
    }
}
