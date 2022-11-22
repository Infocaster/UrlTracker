using Umbraco.Cms.Core.Models.Entities;

namespace UrlTracker.Core.Database.Entities
{
    public interface IRedactionScore
        : IEntity, IRememberBeingDirty
    {
        decimal RedactionScore { get; set; }
    }

    internal class RedactionScoreEntity
        : EntityBase, IRedactionScore
    {
        private decimal _redactionScore;

        public decimal RedactionScore
        {
            get => _redactionScore;
            set => SetPropertyValueAndDetectChanges(value, ref _redactionScore, nameof(RedactionScore));
        }
    }
}
