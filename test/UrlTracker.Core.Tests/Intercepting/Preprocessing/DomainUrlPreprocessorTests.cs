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
        private DomainUrlPreprocessor _testSubject;

        public override void SetUp()
        {
            _testSubject = new DomainUrlPreprocessor(DomainProvider);
        }

        public static IEnumerable<TestCaseData> NormalFlowTestCases()
        {
            string inputUrl = "http://example.com/lorem";
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(0, 1, "example domain", "en-us", Url.Parse("example.com"))
                }),
                "en-us",
                (int?)1,
                inputUrl
            ).SetName("PreprocessUrlAsync sets root node id and culture if url maps to a known domain");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "www example domain", "en-us", Url.Parse("www.example.com"))
                }),
                null,
                null,
                inputUrl
            ).SetName("PreprocessUrlAsync does nothing if the url does not map to a known domain");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "example domain", "en-us", Url.Parse("https://example.com"))
                }),
                null,
                null,
                inputUrl
            ).SetName("PreprocessUrlAsync matches on protocol if provided");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "example domain", "en-us", Url.Parse("/lorem"))
                }),
                "en-us",
                (int?)2,
                inputUrl
            ).SetName("PreprocessUrlAsync matches on relative url");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "example domain", "en-us", Url.Parse("/lorem"))
                }),
                "en-us",
                (int?)2,
                "http://example.com:443/lorem"
            ).SetName("PreprocessUrlAsync ignores port if host is not given");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "example domain", "en-us", Url.Parse("example.com/lorem"))
                }),
                null,
                null,
                "http://example.com:443/lorem"
            ).SetName("PreprocessUrlAsync enforces port if host is given");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "example domain", "en-us", Url.Parse("example.com:443/lorem"))
                }),
                null,
                null,
                "http://example.com:8080/lorem"
            ).SetName("PreprocessUrlAsync refuses domain if port is unequal");
            yield return new TestCaseData
            (
                DomainCollection.Create(new List<Core.Domain.Models.Domain>
                {
                    new Core.Domain.Models.Domain(1, 2, "example domain", "en-us", Url.Parse("example.com:443/lorem"))
                }),
                "en-us",
                (int?)2,
                "http://example.com:443/lorem"
            ).SetName("PreprocessUrlAsync accepts domain of port is equal");
        }

        [TestCaseSource(nameof(NormalFlowTestCases))]
        public async Task PreprocessUrlAsync_NormalFlow_InsertsDomainMatches(DomainCollection domains, string culture, int? rootNodeId, string inputUrl)
        {
            // arrange
            var input = Url.Parse(inputUrl);
            DomainProviderMock.Setup(obj => obj.GetDomains()).Returns(domains).Verifiable();

            // act
            var result = await _testSubject.PreprocessUrlAsync(input, DefaultInterceptContext);

            // assert
            Assert.That(result.GetCulture(), Is.EqualTo(culture));
            Assert.That(result.GetRootNode(), Is.EqualTo(rootNodeId));
        }
    }
}
