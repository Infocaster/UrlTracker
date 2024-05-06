using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Models;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class RequestInterceptFilterCollectionTests
    {
        private Mock<IRequestInterceptFilter> _requestInterceptFilterMock;
        private RequestInterceptFilterCollection? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _requestInterceptFilterMock = new Mock<IRequestInterceptFilter>();
            _testSubject = new RequestInterceptFilterCollection(() => new List<IRequestInterceptFilter> { _requestInterceptFilterMock.Object });
        }

        [TestCase(true, true, TestName = "EvaluateCandidateAsync returns true if all filters return true")]
        [TestCase(false, false, TestName = "EvaluateCandidateAsync returns false if any filter returns false")]
        public async Task EvaluateCandidateAsync_NormalFlow_ReturnsResult(bool output, bool expected)
        {
            // arrange
            _requestInterceptFilterMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>()))
                                      .ReturnsAsync(output);

            // act
            var result = await _testSubject!.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
