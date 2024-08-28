using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Web.Configuration;
using UrlTracker.Web.Processing.Filtering;

namespace UrlTracker.Web.Tests.Processing
{
    public class UrlReservedPathFilterTests
    {
        private Mock<IReservedPathSettingsProvider> _reservedPathSettingsProviderMock;
        private UrlReservedPathFilter? _urlReservedPathFilter;

        [SetUp]
        public void Setup()
        {
            _reservedPathSettingsProviderMock = new Mock<IReservedPathSettingsProvider>();
            _urlReservedPathFilter = new UrlReservedPathFilter(_reservedPathSettingsProviderMock.Object, new VoidLogger<UrlReservedPathFilter>());
        }

        [TestCase("http://example.com/lorem/ipsum", "lorem/ipsum/", false, TestName = "EvaluateCandidateAsync returns false if the url path matches a filter")]
        [TestCase("http://example.com", "lorem/ipsum/", true, TestName = "EvaluateCandidateAsync returns true if the url path does not match any filter")]
        [TestCase("http://example.com/lorem/ipsum", "lorem/", false, TestName = "EvaluateCandidateAsync returns false if the url path starts with any filter")]
        public async Task EvaluateCandidateAsync_NormalFlow_ReturnsMatch(string input, string filteredPath, bool expectation)
        {
            // arrange
            _reservedPathSettingsProviderMock!.Setup(obj => obj.Paths).Returns(new HashSet<string> { filteredPath });
            var inputUri = Url.Parse(input);

            // act
            var result = await _urlReservedPathFilter!.EvaluateCandidateAsync(inputUri);

            // assert
            Assert.That(result, Is.EqualTo(expectation));
        }
    }
}
