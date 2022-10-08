using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Resources.Testing;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class NotFoundClientErrorFilterTests : TestBase
    {
        private NotFoundClientErrorFilter _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new NotFoundClientErrorFilter();
        }

        [TestCase(404, true, TestName = "EvaluateCandidateAsync returns true if response code is 404")]
        [TestCase(302, false, TestName = "EvaluateCandidateAsync returns false if response code is not 404")]
        public async Task EvaluateCandidate_404_ReturnsCorrectResult(int statuscode, bool expected)
        {
            // arrange
            HttpContextMock.ResponseMock.Setup(r => r.StatusCode).Returns(statuscode);

            // act
            var result = await _testSubject.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
