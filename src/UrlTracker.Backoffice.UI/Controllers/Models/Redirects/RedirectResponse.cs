using System;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    [DataContract]
    internal class RedirectResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        [DataMember(Name = "createDate")]
        public DateTime CreateDate { get; set; }

        [DataMember(Name = "sourceStrategy")]
        public Guid SourceStrategy { get; set; }

        [DataMember(Name = "sourceValue")]
        public string SourceValue { get; set; } = null!;

        [DataMember(Name = "targetStrategy")]
        public Guid TargetStrategy { get; set; }

        [DataMember(Name = "targetValue")]
        public string TargetValue { get; set; } = null!;

        [DataMember(Name = "permanent")]
        public bool Permanent { get; set; }

        [DataMember(Name = "retainQuery")]
        public bool RetainQuery { get; set; }

        [DataMember(Name = "force")]
        public bool Force { get; set; }
    }
}
