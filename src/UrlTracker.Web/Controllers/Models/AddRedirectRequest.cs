using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Web.Controllers.Models
{
    public class AddRedirectRequest
        : RedirectRequestBase
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new();
            results.AddRange(base.Validate(validationContext));

            if (Is404 && string.IsNullOrWhiteSpace(OldUrl)) results.Add(new ValidationResult("An old url is required if a client error should be deleted", new[] { nameof(OldUrl) }));

            return results;
        }
    }
}
