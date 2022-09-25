using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

namespace UrlTracker.Backoffice.UI.Tests.Controllers.Models
{
    public abstract class ModelValidationTestBase<TModel> : AssemblyScanTestBase<TModel>
    {
        protected void TryValidateObject_NormalFlow_ValidatesObject(object model, string[] errorsFor, string[] noErrorsFor, bool mustBeValid = false)
        {
            // arrange
            var ctx = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // act
            var result = Validator.TryValidateObject(model, ctx, errors, true);

            // assert
            Assert.Multiple(() =>
            {
                IEnumerable<string> memberNames = errors.SelectMany(e => e.MemberNames);

                Assert.That(memberNames, Is.SupersetOf(errorsFor));
                if (noErrorsFor?.Any() == true) Assert.That(memberNames, Has.No.AnyOf(noErrorsFor));
                if (mustBeValid) Assert.That(result, Is.True);
            });
        }
    }
}
