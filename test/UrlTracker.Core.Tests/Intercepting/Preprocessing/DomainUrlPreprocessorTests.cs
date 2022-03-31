using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Preprocessing;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests.Intercepting.Preprocessing
{
    public class DomainUrlPreprocessorTests : TestBase
    {
        private DomainUrlPreprocessor? _testSubject;

        public override void SetUp()
        {
            _testSubject = new DomainUrlPreprocessor(DomainProvider);
        }

        public static IEnumerable<TestCaseData> NormalFlowTestCases()
        {
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(0, 1, "example domain", "en-us", Url.Parse("example.com"))
                }),
                "en-us",
                (int?)1
            ).SetName("PreprocessUrlAsync sets root node id and culture if url maps to a known domain");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "www example domain", "en-us", Url.Parse("www.example.com"))
                }),
                null,
                null
            ).SetName("PreprocessUrlAsync does nothing if the url does not map to a known domain");
        }

        [TestCaseSource(nameof(NormalFlowTestCases))]
        public async Task PreprocessUrlAsync_NormalFlow_InsertsDomainMatches(DomainCollection domains, string culture, int? rootNodeId)
        {
            // arrange
            var input = Url.Parse("http://example.com/lorem");
            DomainProviderMock!.Setup(obj => obj.GetDomains()).Returns(domains).Verifiable();

            // act
            var result = await _testSubject!.PreprocessUrlAsync(input, DefaultInterceptContext!);

            // assert
            Assert.That(result.GetCulture(), Is.EqualTo(culture));
            Assert.That(result.GetRootNode(), Is.EqualTo(rootNodeId));
        }
    }
}
