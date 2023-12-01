using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Validation.Attributes;

namespace UrlTracker.Core.Models
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class Redirect
        : IValidatableObject
    {
        public string? Notes { get; set; }
        public DateTime Inserted { get; set; }
        // id cannot be validated, because in some cases it's mandatory, but in others it's not
        public int? Id { get; set; }

        [ValidCultureFormat]
        public string? Culture { get; set; }

        [Required]
        public IPublishedContent? TargetRootNode { get; set; } // Might be null if content is taken from an old database or the import function is used.

        public IPublishedContent? TargetNode { get; set; }

        public string? TargetUrl { get; set; }

        public string? SourceUrl { get; set; }

        [ValidRegexPattern]
        public string? SourceRegex { get; set; }

        public bool RetainQuery { get; set; }

        [Range(300, 399)]
        public HttpStatusCode TargetStatusCode { get; set; }

        public bool Force { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(SourceUrl) && string.IsNullOrWhiteSpace(SourceRegex))
            {
                yield return new ValidationResult(Defaults.Validation.SourceConditionNotDefined, new[] { nameof(SourceUrl), nameof(SourceRegex) });
            }
            if (TargetNode is null && string.IsNullOrWhiteSpace(TargetUrl))
            {
                yield return new ValidationResult(Defaults.Validation.TargetConditionNotDefined, new[] { nameof(TargetNode), nameof(TargetUrl) });
            }
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{Id} | {Culture} | {TargetNode?.Id.ToString() ?? TargetUrl} | {TargetStatusCode}";
        }
    }
}
