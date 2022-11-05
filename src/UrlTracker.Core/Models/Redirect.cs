using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Models
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class Redirect
    {
        public DateTime Inserted { get; set; }
        // id cannot be validated, because in some cases it's mandatory, but in others it's not
        public int? Id { get; set; }

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

        private string GetDebuggerDisplay()
        {
            return $"{Id} | {Source} | {Target}";
        }
    }
}
