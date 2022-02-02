using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests
{
    public partial class RedirectServiceTests
    {
        [TestCase(TestName = "UpdateAsync throws ArgumentNullException when redirect is null")]
        public void UpdateAsync_Null_ThrowsException()
        {
            // arrange

            // act
            Task result() => _testSubject.UpdateAsync(null);

            // assert
            AssertArgumentNullException(result, "redirect");
        }

        [TestCase(TestName = "UpdateAsync throws argument exception when argument is invalid")]
        public void UpdateAsync_InvalidArgument_ThrowsException()
        {
            // arrange
            Exception exception = SetupValidationFails();

            // act
            Task result() => _testSubject.UpdateAsync(new Redirect());

            // assert
            AssertValidationException(result, exception);
        }

        [TestCase(TestName = "UpdateAsync does not throw exceptions when argument is valid")]
        public void UpdateAsync_NormalFlow_AddsRedirect()
        {
            // arrange
            SetupValidationSuccessful();

            // act
            Task result() => _testSubject.UpdateAsync(new Redirect { Id = 1000 });

            // assert
            AssertValidationNoExceptions(result);
        }

        [TestCase(TestName = "UpdateAsync throws exception when id is invalid")]
        public void UpdateAsync_InvalidId_ThrowsException()
        {
            // arrange
            SetupValidationSuccessful();

            // act
            Task result() => _testSubject.UpdateAsync(new Redirect());

            // assert
            Assert.Multiple(() =>
            {
                var actualException = Assert.ThrowsAsync<ArgumentException>(result);
                Assert.That(actualException.InnerException, Is.TypeOf<ValidationException>());
            });
        }
    }
}
