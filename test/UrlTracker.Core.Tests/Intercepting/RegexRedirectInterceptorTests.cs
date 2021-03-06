using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class RegexRedirectInterceptorTests : TestBase
    {
        private RegexRedirectInterceptor? _testSubject;

        public override void SetUp()
        {
            _testSubject = new RegexRedirectInterceptor(RedirectRepository, new ConsoleLogger<RegexRedirectInterceptor>());
            DefaultInterceptContext!.SetRootNode(9999);
        }

        public static IEnumerable<TestCaseData> NormalFlowTestCases()
        {
            var entry1 = new UrlTrackerShallowRedirect
            {
                SourceRegex = @"\/ipsum"
            };

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

            var entry2 = new UrlTrackerShallowRedirect
            {
                SourceRegex = @"^lorem\?ipsum=[0-9]{3}$"
            };

            yield return new TestCaseData(
                entry2,
                Url.Parse("http://example.com/lorem?ipsum=123"),
                entry2).SetName("InterceptAsync matches on query string");

            var entry3 = new UrlTrackerShallowRedirect
            {
                SourceRegex = "lorem",
                TargetRootNodeId = 9998
            };
            yield return new TestCaseData(
                entry3,
                Url.Parse("http://example.com/lorem"),
                null).SetName("InterceptAsync does not match if target root node is unequal to request root node");
        }

        [TestCaseSource(nameof(NormalFlowTestCases))]
        public async Task InterceptAsync_NormalFlow_ReturnsMatch(UrlTrackerShallowRedirect redirect, Url input, UrlTrackerShallowRedirect expected)
        {
            // arrange
            RedirectRepositoryMock!.Setup(obj => obj.GetShallowWithRegexAsync())
                                  .ReturnsAsync(new List<UrlTrackerShallowRedirect> { redirect });

            // act
            var result = await _testSubject!.InterceptAsync(input, DefaultInterceptContext!);

            // assert
            Assert.That(result?.Info, Is.EqualTo(expected));
        }
    }
}
