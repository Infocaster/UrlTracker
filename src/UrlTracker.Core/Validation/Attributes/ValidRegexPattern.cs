using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UrlTracker.Core.Validation.Attributes
{
    /// <summary>
    /// Used for specifying that a value must be a valid regex pattern.
    /// </summary>
    public class ValidRegexPattern : ValidationAttribute
    {
        /// <inheritdoc/>
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = Convert.ToString(value);

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return ValidationResult.Success!;
            }

            try
            {
                Regex.Match(string.Empty, stringValue);
            }
            catch (ArgumentException)
            {
                return new ValidationResult(Defaults.Validation.RegexConditionInvalidFormat);
            }

            return ValidationResult.Success!;
        }
    }
}