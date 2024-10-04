using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace UrlTracker.Core.Validation.Attributes
{
    public class ValidCultureFormat : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.Success;
            }

            try
            {
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(stringValue);
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(Defaults.Validation.CultureConditionInvalidFormat);
            }
        }
    }
}
