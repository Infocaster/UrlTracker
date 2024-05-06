using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests
{
    public class InterceptServiceTests
    {
        private Mock<IIntermediateInterceptService>? _intermediateInterceptServiceMock;
        private Mock<IInterceptConverterCollection>? _interceptConverterCollectionMock;
        private InterceptService? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _intermediateInterceptServiceMock = new Mock<IIntermediateInterceptService>();
            _interceptConverterCollectionMock = new Mock<IInterceptConverterCollection>();
            _testSubject = new InterceptService(_intermediateInterceptServiceMock.Object, _interceptConverterCollectionMock.Object);
        }

        [TestCase(TestName = "GetAsync with absolute url does not throw exceptions")]
        public void GetAsync_NormalFlow_ReturnsResult()
        {
            // arrange
            ICachableIntercept output = new CachableInterceptBase<object>(new object());
            _intermediateInterceptServiceMock!.Setup(obj => obj.GetAsync(It.IsAny<Url>(), null)).ReturnsAsync(output);
            _interceptConverterCollectionMock!.Setup(obj => obj.ConvertAsync(output)).ReturnsAsync((ICachableIntercept c) => c);

            // act
            Task result() => _testSubject!.GetAsync(Url.Parse("http://example.com/lorem"));

            // assert
            Assert.DoesNotThrowAsync(result);
        }

        [TestCase(TestName = "GetAsync with relative url throws argument exception")]
        public void GetAsyncUrl_RelativeUrl_ThrowsException()
        {
            // arrange
            var input = Url.Parse("/lorem");

            // act
            Task result() => _testSubject!.GetAsync(input);

            // assert
            Assert.Multiple(() =>
            {
                var ex = Assert.ThrowsAsync<ArgumentException>(result);
                Assert.That(ex?.ParamName, Is.EqualTo("url"));
            });
        }
    }
}
