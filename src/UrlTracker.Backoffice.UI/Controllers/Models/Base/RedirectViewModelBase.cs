using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Base
{
    [DataContract]
    public abstract class RedirectViewModelBase
    {
        [Required]
        [DataMember(Name = "source")]
        public StrategyViewModel Source { get; set; } = null!;

        [Required]
        [DataMember(Name = "target")]
        public StrategyViewModel Target { get; set; } = null!;

        [Required]
        [DataMember(Name = "permanent")]
        public bool Permanent { get; set; }

        [Required]
        [DataMember(Name = "retainQuery")]
        public bool RetainQuery { get; set; }

        [Required]
        [DataMember(Name = "force")]
        public bool Force { get; set; }

        [DataMember(Name = "key")]
        public Guid? Key { get; set; }
    }
}