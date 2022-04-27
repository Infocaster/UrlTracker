using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using UrlTracker.Core.Validation;

namespace UrlTracker.Core.Tests.Validation
{
    public class ValidationHelperTests
    {
        private ValidationHelper _testSubject;

        [SetUp]
        public void SetUp()
        {
            _testSubject = new ValidationHelper();
        }

        [TestCase(TestName = "EnsureValidObject throws validation exception if the object is not valid")]
        public void EnsureValidObject_InvalidObject_ThrowsException()
        {
            // arrange
            var input = new TestValidationObject();

            // act
            void result() => _testSubject.EnsureValidObject(input);

            // assert
            Assert.Throws<ValidationException>(result);
        }

        [TestCase(TestName = "EnsureValidObject does not throw an exception if the object is valid")]
        public void EnsureValidObject_ValidObject_DoesNotThrowException()
        {
            // arrange
            var input = new TestValidationObject
            {
                Parameter = "test"
            };

            // act
            void result() => _testSubject.EnsureValidObject(input);

            // assert
            Assert.DoesNotThrow(result);
        }
    }

    public class TestValidationObject
    {
        [Required]
        public string Parameter { get; set; }
    }
}
