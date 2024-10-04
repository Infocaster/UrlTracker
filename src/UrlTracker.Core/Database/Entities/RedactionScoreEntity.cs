using Umbraco.Cms.Core.Models.Entities;

namespace UrlTracker.Core.Database.Entities
{
    public interface IRedactionScore
        : IEntity, IRememberBeingDirty
    {
        string? Name { get; set; }
        decimal RedactionScore { get; set; }
    }

    internal class RedactionScoreEntity
        : EntityBase, IRedactionScore
    {
        private string? _name;
        private decimal _redactionScore;

        public string? Name
        {
            get => _name;
            set => SetPropertyValueAndDetectChanges(value, ref _name, nameof(Name));
        }

        public decimal RedactionScore
        {
            get => _redactionScore;
            set => SetPropertyValueAndDetectChanges(value, ref _redactionScore, nameof(RedactionScore));
        }
    }
}
