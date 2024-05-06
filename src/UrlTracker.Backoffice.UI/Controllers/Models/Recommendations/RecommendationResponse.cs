using System;
using System.Runtime.Serialization;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations
{
    [DataContract]
    internal record RecommendationResponse(
        [property: DataMember(Name = "id")] int Id,
        [property: DataMember(Name = "ignore")] bool Ignore,
        [property: DataMember(Name = "url")] string Url,
        [property: DataMember(Name = "strategy")] Guid Strategy,
        [property: DataMember(Name = "score")] int VariableScore,
        [property: DataMember(Name = "updatedate")] DateTime UpdateDate)
    {
        public static RecommendationResponse FromEntity(IRecommendation entity)
            => new(
                entity.Id,
                entity.Ignore,
                entity.Url,
                entity.Strategy.Key,
                entity.VariableScore,
                entity.UpdateDate);
    }
}
