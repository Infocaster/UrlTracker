using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Models.Entities;

namespace UrlTracker.Core.Database.Entities
{
    public interface IClientError : IEntity, IRememberBeingDirty
    {
        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public bool Ignored { get; set; }

        [DataMember]
        public Guid Strategy { get; set; }

        public int TotalOccurrences { get; }

        public string? MostCommonReferrer { get; }

        public DateTime MostRecentOccurrence { get; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    [ExcludeFromCodeCoverage]
    public class ClientErrorEntity : EntityBase, IClientError
    {
        private Guid _strategy;
        private string _url;
        private bool _ignored;

        public ClientErrorEntity(string url, bool ignored, Guid strategy, int totalOccurrences, string? mostCommonReferrer, DateTime mostRecentOccurrence)
        {
            _url = url;
            _ignored = ignored;
            _strategy = strategy;
            TotalOccurrences = totalOccurrences;
            MostCommonReferrer = mostCommonReferrer;
            MostRecentOccurrence = mostRecentOccurrence;
        }

        public ClientErrorEntity(string url, bool ignore, Guid strategy)
            : this(url, ignore, strategy, default, default, default)
        { }

        public string Url
        {
            get => _url;
            set => SetPropertyValueAndDetectChanges(value, ref _url!, nameof(Url));
        }

        public bool Ignored
        {
            get => _ignored;
            set => SetPropertyValueAndDetectChanges(value, ref _ignored, nameof(Ignored));
        }

        public Guid Strategy
        {
            get => _strategy;
            set => SetPropertyValueAndDetectChanges(value, ref _strategy, nameof(Strategy));
        }

        public int TotalOccurrences { get; }

        public string? MostCommonReferrer { get; }

        public DateTime MostRecentOccurrence { get; }

        private string GetDebuggerDisplay()
        {
            return Url + (Ignored ? " (ignored)" : string.Empty);
        }
    }
}
