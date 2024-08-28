using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class InterceptorCollectionTests
    {
        private Mock<IInterceptor> _interceptorMock;
        private InterceptorCollection? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _interceptorMock = new Mock<IInterceptor>();
            _testSubject = new InterceptorCollection(() => new List<IInterceptor> { _interceptorMock.Object }, new NullInterceptor());
        }

        public static TestCaseData[] TestCases()
        {
            var intercept = new CachableInterceptBase<object>(new object());
            return new TestCaseData[]
            {
                new TestCaseData(intercept, intercept).SetName("InterceptAsync returns intercept if one is found"),
                new TestCaseData(null, CachableInterceptBase.NullIntercept).SetName("InterceptAsync returns default fallback if none is found")
            };
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task InterceptAsync_NormalFlow_ReturnsExpectedResult(ICachableIntercept input, ICachableIntercept expected)
        {
            // arrange
            _interceptorMock!.Setup(obj => obj.InterceptAsync(It.IsAny<Url>(), It.IsAny<IInterceptContext>()))
                           .ReturnsAsync(input);

            // act
            var result = await _testSubject!.InterceptAsync(Url.Parse("https://example.com"), new DefaultInterceptContext());

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
