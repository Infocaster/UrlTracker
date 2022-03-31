using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UrlTracker.Core.Tests
{
    public partial class ClientErrorServiceTests
    {
        [TestCase(TestName = "DeleteAsync throws argument exception if url is null or whitespace")]
        public void DeleteAsync_UrlIsNullOrWhiteSpace_ThrowsException()
        {
            // arrange

            // act
            Task result() => _testSubject!.DeleteAsync(string.Empty, "nl-nl");

            // assert
            Assert.Multiple(() =>
            {
                var actualException = Assert.ThrowsAsync<ArgumentException>(result);
                Assert.That(actualException?.ParamName, Is.EqualTo("url"));
            });
        }

        [TestCase(TestName = "DeleteAsync does not throw exceptions when valid parameters are passed")]
        public void DeleteAsync_CultureIsEmpty_EnsuresCultureIsNull()
        {
            // arrange

            // act
            Task result() => _testSubject!.DeleteAsync("http://example.com", string.Empty);

            // assert
            Assert.DoesNotThrowAsync(result);
        }
    }
}
