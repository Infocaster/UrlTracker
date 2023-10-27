using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Validation.Attributes
{
    public class ValidCultureFormat : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Redirect redirect = (Redirect)validationContext.ObjectInstance;

            try
            {
                if(redirect.Culture == null)
                {
                    return ValidationResult.Success;
                }
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(redirect.Culture);
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(Defaults.Validation.CultureConditionInvalidFormat, new[] { nameof(redirect.Culture) });
            }
        }
    }
}
