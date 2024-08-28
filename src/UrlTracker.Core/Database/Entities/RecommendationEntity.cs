using Umbraco.Cms.Core.Models.Entities;

namespace UrlTracker.Core.Database.Entities
{
    public interface IRecommendation
        : IEntity, IRememberBeingDirty
    {
        int StrategyId { get; }
        IRedactionScore Strategy { get; set; }
        bool Ignore { get; set; }
        int VariableScore { get; set; }
        string Url { get; set; }
    }

    internal class RecommendationEntity
        : EntityBase, IRecommendation
    {
        private IRedactionScore _strategy;
        private bool _ignore;
        private int _variableScore;
        private string _url;

        public RecommendationEntity(string url, IRedactionScore strategy)
        {
            _url = url;
            _strategy = strategy;
        }

        public int StrategyId => Strategy.Id;

        public IRedactionScore Strategy
        {
            get => _strategy;
            set => SetPropertyValueAndDetectChanges(value, ref _strategy!, nameof(Strategy));
        }

        public bool Ignore
        {
            get => _ignore;
            set => SetPropertyValueAndDetectChanges(value, ref _ignore, nameof(Ignore));
        }

        public int VariableScore
        {
            get => _variableScore;
            set => SetPropertyValueAndDetectChanges(value, ref _variableScore, nameof(VariableScore));
        }

        public string Url
        {
            get => _url;
            set => SetPropertyValueAndDetectChanges(value, ref _url!, nameof(Url));
        }
    }
}
