using System;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Scoring
{
    [DataContract]
    internal record RedactionScoreResponse(
        [property: DataMember(Name = "key")] Guid Key,
        [property: DataMember(Name = "score")] decimal Score);
}
