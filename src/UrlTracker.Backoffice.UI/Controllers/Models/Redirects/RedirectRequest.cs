using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    /// <summary>
    /// The request model that specifies defining features of a redirect without specifying a redirect in particular
    /// </summary>
    [DataContract]
    public class RedirectRequest
    {
        /// <summary>
        /// The strategy to use when matching incoming urls
        /// </summary>
        [Required]
        [DataMember(Name = "sourceStrategy")]
        public Guid SourceStrategy { get; set; }

        /// <summary>
        /// The parameters to use for the given strategy
        /// </summary>
        [Required, MaxLength(255)]
        [DataMember(Name = "sourceValue")]
        public string SourceValue { get; set; } = null!;

        /// <summary>
        /// The strategy to use when building outgoing URLs
        /// </summary>
        [Required]
        [DataMember(Name = "targetStrategy")]
        public Guid TargetStrategy { get; set; }

        /// <summary>
        /// The parameters to use for the given strategy
        /// </summary>
        [Required, MaxLength(255)]
        [DataMember(Name = "targetValue")]
        public string TargetValue { get; set; } = null!;

        /// <summary>
        /// Set this parameter to <see langword="true" /> to make the redirect permanent
        /// </summary>
        [Required]
        [DataMember(Name = "permanent")]
        public bool Permanent { get; set; }

        /// <summary>
        /// Set this parameter to <see langword="true" /> to include the query parameters from the incoming request to the outgoing URL
        /// </summary>
        [Required]
        [DataMember(Name = "retainQuery")]
        public bool RetainQuery { get; set; }

        /// <summary>
        /// Set this parameter to <see langword="true" /> to perform this redirect regardless of the original response 
        /// </summary>
        [Required]
        [DataMember(Name = "force")]
        public bool Force { get; set; }
    }
}
