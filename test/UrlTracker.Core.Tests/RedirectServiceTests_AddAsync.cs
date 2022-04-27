using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests
{
    public partial class RedirectServiceTests
    {
        [TestCase(TestName = "AddAsync throws ArgumentNullException when redirect is null")]
        public void AddAsync_Null_ThrowsException()
        {
            // arrange

            // act
            Task result() => _testSubject.AddAsync(null);

            // assert
            AssertArgumentNullException(result, "redirect");
        }

        [TestCase(TestName = "AddAsync throws argument exception when argument is invalid")]
        public void AddAsync_InvalidArgument_ThrowsException()
        {
            // arrange
            Exception exception = SetupValidationFails();

            // act
            Task result() => _testSubject.AddAsync(new Redirect());

            // assert
            AssertValidationException(result, exception);
        }

        [TestCase(TestName = "AddAsync does not throw exceptions when argument is valid")]
        public void AddAsync_NormalFlow_AddsRedirect()
        {
            // arrange
            SetupValidationSuccessful();

            // act
            Task result() => _testSubject.AddAsync(new Redirect());

            // assert
            AssertValidationNoExceptions(result);
        }
    }
}
