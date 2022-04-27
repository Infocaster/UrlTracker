using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Core.Validation
{
    public class ValidationHelper
        : IValidationHelper
    {
        public void EnsureValidObject(object obj)
        {
            var validationContext = new ValidationContext(obj);
            Validator.ValidateObject(obj, validationContext, true);
        }
    }
}
