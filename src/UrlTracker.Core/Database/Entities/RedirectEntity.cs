using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Umbraco.Core.Models.Entities;

namespace UrlTracker.Core.Database.Models.Entities
{
    public interface IRedirect
        : IEntity, IRememberBeingDirty
    {
        [DataMember]
        string Culture { get; set; }

        [DataMember]
        int? TargetRootNodeId { get; set; }

        [DataMember]
        int? TargetNodeId { get; set; }

        [DataMember]
        string TargetUrl { get; set; }

        [DataMember]
        string SourceUrl { get; set; }

        [DataMember]
        string SourceRegex { get; set; }

        [DataMember]
        bool RetainQuery { get; set; }

        [DataMember]
        bool Permanent { get; set; }

        [DataMember]
        bool Force { get; set; }

        [DataMember]
        string Notes { get; set; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class RedirectEntity : EntityBase, IRedirect
    {
        private string _culture;
        private int? _targetRootNodeId;
        private int? _targetNodeId;
        private string _targetUrl;
        private string _sourceUrl;
        private string _sourceRegex;
        private bool _retainQuery;
        private bool _permanent;
        private bool _force;
        private string _notes;

        public RedirectEntity(string culture, int? targetRootNodeId, int? targetNodeId, string targetUrl, string sourceUrl, string sourceRegex, bool retainQuery, bool permanent, bool force, string notes)
        {
            _culture = culture;
            _targetRootNodeId = targetRootNodeId;
            _targetNodeId = targetNodeId;
            _targetUrl = targetUrl;
            _sourceUrl = sourceUrl;
            _sourceRegex = sourceRegex;
            _retainQuery = retainQuery;
            _permanent = permanent;
            _force = force;
            _notes = notes;
        }

        public string Culture
        {
            get => _culture;
            set => SetPropertyValueAndDetectChanges(value, ref _culture, nameof(Culture));
        }

        public int? TargetRootNodeId
        {
            get => _targetRootNodeId;
            set => SetPropertyValueAndDetectChanges(value, ref _targetRootNodeId, nameof(TargetRootNodeId));
        }

        public int? TargetNodeId
        {
            get => _targetNodeId;
            set => SetPropertyValueAndDetectChanges(value, ref _targetNodeId, nameof(TargetNodeId));
        }

        public string TargetUrl
        {
            get => _targetUrl;
            set => SetPropertyValueAndDetectChanges(value, ref _targetUrl, nameof(TargetUrl));
        }

        public string SourceUrl
        {
            get => _sourceUrl;
            set => SetPropertyValueAndDetectChanges(value, ref _sourceUrl, nameof(SourceUrl));
        }

        public string SourceRegex
        {
            get => _sourceRegex;
            set => SetPropertyValueAndDetectChanges(value, ref _sourceRegex, nameof(SourceRegex));
        }

        public bool RetainQuery
        {
            get => _retainQuery;
            set => SetPropertyValueAndDetectChanges(value, ref _retainQuery, nameof(RetainQuery));
        }

        public bool Permanent
        {
            get => _permanent;
            set => SetPropertyValueAndDetectChanges(value, ref _permanent, nameof(Permanent));
        }

        public bool Force
        {
            get => _force;
            set => SetPropertyValueAndDetectChanges(value, ref _force, nameof(Force));
        }

        public string Notes
        {
            get => _notes;
            set => SetPropertyValueAndDetectChanges(value, ref _notes, nameof(Notes));
        }

        private string GetDebuggerDisplay()
        {
            return $"From {SourceUrl ?? SourceRegex} to {TargetNodeId?.ToString() ?? TargetUrl}";
        }
    }
}
