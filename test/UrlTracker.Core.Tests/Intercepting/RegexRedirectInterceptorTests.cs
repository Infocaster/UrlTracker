using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Models.Entities;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class RegexRedirectInterceptorTests : TestBase
    {
        private RegexRedirectInterceptor _testSubject;

        public override void SetUp()
        {
            _testSubject = new RegexRedirectInterceptor(RedirectRepository, new VoidLogger());
            DefaultInterceptContext.SetRootNode(9999);
        }

        public static IEnumerable<TestCaseData> NormalFlowTestCases()
        {
            var entry1 = new RedirectEntity(default, default, default, default, default, @"\/ipsum", default, default, default, default);

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

            var entry2 = new RedirectEntity(default, default, default, default, default, @"^lorem\?ipsum=[0-9]{3}$", default, default, default, default);

            yield return new TestCaseData(
                entry2,
                Url.Parse("http://example.com/lorem?ipsum=123"),
                entry2).SetName("InterceptAsync matches on query string");

            var entry3 = new RedirectEntity(default, 9998, default, default, default, "lorem", default, default, default, default);
            yield return new TestCaseData(
                entry3,
                Url.Parse("http://example.com/lorem"),
                null).SetName("InterceptAsync does not match if target root node is unequal to request root node");
        }

        [TestCaseSource(nameof(NormalFlowTestCases))]
        public async Task InterceptAsync_NormalFlow_ReturnsMatch(IRedirect redirect, Url input, IRedirect expected)
        {
            // arrange
            RedirectRepositoryMock.Setup(obj => obj.GetWithRegexAsync())
                                  .ReturnsAsync(new List<IRedirect> { redirect });

            // act
            var result = await _testSubject.InterceptAsync(input, DefaultInterceptContext);

            // assert
            Assert.That(result?.Info, Is.EqualTo(expected));
        }
    }
}
