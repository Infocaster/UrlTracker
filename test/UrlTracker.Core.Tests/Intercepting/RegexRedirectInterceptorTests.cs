using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class RegexRedirectInterceptorTests
    {
        private RegexRedirectInterceptor? _testSubject;
        private Mock<IRedirectRepository>? _redirectRepositoryMock;
        private DefaultInterceptContext? _defaultInterceptContext;

        [SetUp]
        public void SetUp()
        {
            _redirectRepositoryMock = new Mock<IRedirectRepository>();
            _defaultInterceptContext = new DefaultInterceptContext();
            _testSubject = new RegexRedirectInterceptor(_redirectRepositoryMock.Object, new VoidLogger<RegexRedirectInterceptor>());
        }

        public static IEnumerable<TestCaseData> NormalFlowTestCases()
        {
            var entry1 = new RedirectEntity(default, default, default, EntityStrategy.RegexSource(@"\/ipsum"), EntityStrategy.UrlTarget("https://example.com"));

            yield return new TestCaseData(
                entry1,
                Url.Parse("http://example.com/lorem/ipsum"),
                entry1
                ).SetName("InterceptAsync returns match if path matches regex");

            yield return new TestCaseData(
                entry1,
                Url.Parse("http://example.com/ipsum"),
                null
                ).SetName("InterceptAsync does not match on leading slash");

            yield return new TestCaseData(
                entry1,
                Url.Parse("http://example.com/lorem"),
                null
                ).SetName("InterceptAsync returns null if path does not match regex");

            var entry2 = new RedirectEntity(default, default, default, EntityStrategy.RegexSource(@"^lorem\?ipsum=[0-9]{3}$"), EntityStrategy.UrlTarget("https://example.com"));

            yield return new TestCaseData(
                entry2,
                Url.Parse("http://example.com/lorem?ipsum=123"),
                entry2).SetName("InterceptAsync matches on query string");
        }

        [TestCaseSource(nameof(NormalFlowTestCases))]
        public async Task InterceptAsync_NormalFlow_ReturnsMatch(IRedirect redirect, Url input, IRedirect expected)
        {
            // arrange
            _redirectRepositoryMock!.Setup(obj => obj.GetWithRegexAsync())
                                  .ReturnsAsync(new List<IRedirect> { redirect });

            // act
            var result = await _testSubject!.InterceptAsync(input, _defaultInterceptContext!);

            // assert
            Assert.That(result?.Info, Is.EqualTo(expected));
        }
    }
}
