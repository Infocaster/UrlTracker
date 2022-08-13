using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Models.Entities;

namespace UrlTracker.Core.Database.Entities
{
    public interface IReferrer
        : IEntity, IRememberBeingDirty
    {
        [DataMember]
        public string Url { get; set; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class ReferrerEntity
        : EntityBase, IReferrer
    {
        private string _url;

        public ReferrerEntity(string url)
        {
            _url = url;
        }

        public string Url
        {
            get => _url;
            set => SetPropertyValueAndDetectChanges(value, ref _url!, nameof(Url));
        }

        private string GetDebuggerDisplay()
        {
            return Url;
        }
    }
}
