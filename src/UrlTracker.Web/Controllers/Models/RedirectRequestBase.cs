using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UrlTracker.Core;

namespace UrlTracker.Web.Controllers.Models
{
    // FIXME: validation clauses taken directly from old code, these may be wrong or incomplete
    public abstract class RedirectRequestBase
        : IValidatableObject
    {
        public string Culture { get; set; }

        public string OldUrl { get; set; }

        public string OldRegex { get; set; }

        public int RedirectRootNodeId { get; set; }

        public int? RedirectNodeId { get; set; }

        public string RedirectUrl { get; set; }

        [Required, Range(300, 399)]
        public int RedirectHttpCode { get; set; }

        public bool RedirectPassThroughQueryString { get; set; }

        public string Notes { get; set; }

        public bool Is404 { get; set; }

        public bool Remove404 { get; set; }

        public string Referrer { get; set; }

        public int? Occurrences { get; set; }

        public DateTime Inserted { get; set; }

        public bool ForceRedirect { get; set; }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(OldUrl) && string.IsNullOrWhiteSpace(OldRegex))
            {
                yield return new ValidationResult(Defaults.Validation.SourceConditionNotDefined, new[] { nameof(OldUrl), nameof(OldRegex) });
            }

            if ((RedirectNodeId ?? 0) < 1 && string.IsNullOrWhiteSpace(RedirectUrl))
            {
                yield return new ValidationResult(Defaults.Validation.TargetConditionNotDefined, new[] { nameof(RedirectNodeId), nameof(RedirectUrl) });
            }

            // HACK: In the old code, there was 1 exception on the requirement for a redirectrootnode:
            //   RedirectRootNodeId can be -1 when adding a redirect AND removing 404s
            //   This exception should be removed when the frontend is updated
            if (RedirectRootNodeId <= 0 && !Remove404)
            {
                yield return new ValidationResult(Defaults.Validation.RedirectRootNodeIdNotSpecified, new[] { nameof(RedirectRootNodeId) });
            }
        }
    }
}