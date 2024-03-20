using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Umbraco.Core.Models.Entities;

namespace UrlTracker.Core.Database.Entities
{
    public interface IClientError : IEntity, IRememberBeingDirty
    {
        [DataMember]
        string Url { get; set; }

        [DataMember]
        bool Ignored { get; set; }

        [DataMember]
        Guid Strategy { get; set; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class ClientErrorEntity : EntityBase, IClientError
    {
        private Guid _strategy;
        private string _url;
        private bool _ignored;

        public ClientErrorEntity(string url, bool ignored, Guid strategy)
        {
            _url = url;
            _ignored = ignored;
            _strategy = strategy;
        }

        public string Url
        {
            get => _url;
            set => SetPropertyValueAndDetectChanges(value, ref _url, nameof(Url));
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

        private string GetDebuggerDisplay()
        {
            return Url + (Ignored ? " (ignored)" : string.Empty);
        }
    }
}
