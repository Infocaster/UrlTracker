using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Models
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class Redirect : IValidatableObject
    {
        public DateTime Inserted { get; set; }
        // id cannot be validated, because in some cases it's mandatory, but in others it's not
        public int? Id { get; set; }

        public Guid? Key { get; set; }

        [Required]
        public bool RetainQuery { get; set; }

        [Required]
        public bool Permanent { get; set; }

        [Required]
        public bool Force { get; set; }

        [Required]
        public ISourceStrategy Source { get; set; }

        [Required]
        public ITargetStrategy Target { get; set; }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new();
            Validator.TryValidateObject(Source, new ValidationContext(Source, validationContext, validationContext.Items), validationResults, true);
            Validator.TryValidateObject(Target, new ValidationContext(Target, validationContext, validationContext.Items), validationResults, true);

            return validationResults;
        }

        private string GetDebuggerDisplay()
        {
            return $"{Id} | {Source} | {Target}";
        }
    }
}
