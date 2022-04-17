using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class InterceptorCollectionTests : TestBase
    {
        private InterceptorCollection _testSubject;

        public override void SetUp()
        {
            _testSubject = new InterceptorCollection(new List<IInterceptor> { Interceptor }, new NullInterceptor());
        }

        public static TestCaseData[] TestCases()
        {
            var intercept = new CachableInterceptBase<object>(new object());
            return new TestCaseData[]
            {
                new TestCaseData(intercept, intercept).SetName("InterceptAsync returns intercept if one is found"),
                new TestCaseData(null, CachableInterceptBase.NullIntercept).SetName("InterceptAsync returns NullIntercept if none is found")
            };
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task InterceptAsync_NormalFlow_ReturnsExpectedResult(ICachableIntercept input, ICachableIntercept expected)
        {
            // arrange
            InterceptorMock.Setup(obj => obj.InterceptAsync(It.IsAny<Url>(), It.IsAny<IReadOnlyInterceptContext>()))
                           .ReturnsAsync(input);

            // act
            var result = await _testSubject.InterceptAsync(Url.Parse("https://example.com"), new DefaultInterceptContext());

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
