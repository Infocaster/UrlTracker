using System;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations
{
    [DataContract]
    internal record RecommendationResponse(
        [property: DataMember(Name = "id")] int Id,
        [property: DataMember(Name = "ignore")] bool Ignore,
        [property: DataMember(Name = "url")] string Url,
        [property: DataMember(Name = "strategy")] Guid Strategy,
        [property: DataMember(Name = "score")] int VariableScore,
        [property: DataMember(Name = "updatedate")] DateTime UpdateDate);
}
