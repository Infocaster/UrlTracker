using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class UrlReservedPathFilterTests : TestBase
    {
        private UrlReservedPathFilter _urlReservedPathFilter;

        [SetUp]
        public void Setup()
        {
            _urlReservedPathFilter = new UrlReservedPathFilter(ReservedPathSettingsProvider, new ConsoleLogger<UrlReservedPathFilter>());
        }

        [TestCase("http://example.com/lorem/ipsum", "lorem/ipsum/", false, TestName = "EvaluateCandidateAsync returns false if the url path matches a filter")]
        [TestCase("http://example.com", "lorem/ipsum/", true, TestName = "EvaluateCandidateAsync returns true if the url path does not match any filter")]
        [TestCase("http://example.com/lorem/ipsum", "lorem/", false, TestName = "EvaluateCandidateAsync returns true if the url path starts with any filter")]
        public async Task EvaluateCandidateAsync_NormalFlow_ReturnsMatch(string input, string filteredPath, bool expectation)
        {
            // arrange
            ReservedPathSettingsProviderMock.Setup(obj => obj.Paths).Returns(new HashSet<string> { filteredPath });
            var inputUri = Url.Parse(input);

            // act
            var result = await _urlReservedPathFilter.EvaluateCandidateAsync(inputUri);

            // assert
            Assert.That(result, Is.EqualTo(expectation));
        }
    }
}
