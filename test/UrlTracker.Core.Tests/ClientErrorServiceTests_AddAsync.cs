using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests
{
    public partial class ClientErrorServiceTests
    {
        [TestCase(TestName = "AddAsync throws argument null exception if argument is null")]
        public void AddAsync_ArgumentNull_ThrowsException()
        {
            // arrange

            // act
            Task result() => _testSubject!.AddAsync(null!);

            // assert
            Assert.ThrowsAsync<ArgumentNullException>(result);
        }

        [TestCase(TestName = "AddAsync throws argument exception on invalid models")]
        public void AddAsync_InvalidParameter_ThrowsException()
        {
            // arrange
            var exception = new Exception();
            _validationHelperMock!.Setup(obj => obj.EnsureValidObject(It.IsAny<ClientError>())).Throws(exception);

            // act
            Task result() => _testSubject!.AddAsync(new ClientError("http://example.com"));

            // assert
            Assert.Multiple(() =>
            {
                var actualException = Assert.ThrowsAsync<ArgumentException>(result);
                Assert.That(actualException?.InnerException, Is.SameAs(exception));
            });
        }

        [TestCase(TestName = "AddAsync does not throw exceptions when a valid model is passed")]
        public void AddAsync_NormalFlow_DoesNotThrow()
        {
            // arrange
            _validationHelperMock!.Setup(obj => obj.EnsureValidObject(It.IsAny<ClientError>())).Verifiable();

            // act
            Task result() => _testSubject.AddAsync(new ClientError("http://example.com"));

            // assert
            Assert.DoesNotThrowAsync(result);
            _validationHelperMock.Verify();
        }
    }
}
