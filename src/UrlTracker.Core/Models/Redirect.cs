﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Umbraco.Core.Models.PublishedContent;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Models
{
    /* Why the difference?
     * 
     * Shallow redirects are explicitly for intercepting and are supposed to be cached.
     * The cache should be as small as possible, therefore the shallow object contains
     *    only those elements that are required for caching and redirecting
     */

    public class ShallowRedirect
        : IValidatableObject
    {
        // id cannot be validated, because in some cases it's mandatory, but in others it's not
        public int? Id { get; set; }

        public string Culture { get; set; }

        [Required]
        public IPublishedContent TargetRootNode { get; set; }

        public IPublishedContent TargetNode { get; set; }

        public string TargetUrl { get; set; }

        public Url SourceUrl { get; set; }

        public string SourceRegex { get; set; }

        public bool PassThroughQueryString { get; set; }

        [Range(300, 399)]
        public HttpStatusCode TargetStatusCode { get; set; }

        public bool Force { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SourceUrl is null && string.IsNullOrWhiteSpace(SourceRegex))
            {
                yield return new ValidationResult(Defaults.Validation.SourceConditionNotDefined, new[] { nameof(SourceUrl), nameof(SourceRegex) });
            }
            if (TargetNode is null && string.IsNullOrWhiteSpace(TargetUrl))
            {
                yield return new ValidationResult(Defaults.Validation.TargetConditionNotDefined, new[] { nameof(TargetNode), nameof(TargetUrl) });
            }
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class Redirect
        : ShallowRedirect
    {
        public string Notes { get; set; }
        public DateTime Inserted { get; set; }

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