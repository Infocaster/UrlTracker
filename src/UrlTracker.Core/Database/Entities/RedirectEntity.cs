using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Models.Entities;

namespace UrlTracker.Core.Database.Entities
{
    /// <summary>
    /// When implemented, this type provides model values for a redirect
    /// </summary>
    public interface IRedirect
        : IEntity, IRememberBeingDirty
    {
        /// <summary>
        /// If true, query string parameters on the incoming URL should transfer to the outgoing URL
        /// </summary>
        [DataMember]
        bool RetainQuery { get; set; }

        /// <summary>
        /// If true, the redirect should use the permanent redirect HTTP Status code
        /// </summary>
        [DataMember]
        bool Permanent { get; set; }

        /// <summary>
        /// If true, the redirect should apply, regardless of the original response
        /// </summary>
        [DataMember]
        bool Force { get; set; }

        /// <summary>
        /// The strategy by which to match incoming URLs
        /// </summary>
        [DataMember]
        EntityStrategy Source { get; set; }

        /// <summary>
        /// The strategy by which to construct outgoing URLs
        /// </summary>
        [DataMember]
        EntityStrategy Target { get; set; }

        /// <summary>
        /// Flattened strategy identifier for database query builder
        /// </summary>
        Guid SourceStrategy { get; }

        /// <summary>
        /// Flattened strategy value for database query builder
        /// </summary>
        string SourceValue { get; }

        /// <summary>
        /// Flattened strategy identifier for database query builder
        /// </summary>
        Guid TargetStrategy { get; }

        /// <summary>
        /// Flattened strategy value for database query builder
        /// </summary>
        string TargetValue { get; }
    }

    /// <summary>
    /// Default implementation of <see cref="IRedirect" />
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    [ExcludeFromCodeCoverage]
    public class RedirectEntity : EntityBase, IRedirect
    {
        private bool _retainQuery;
        private bool _permanent;
        private bool _force;
        private EntityStrategy _source;
        private EntityStrategy _target;

        /// <inheritdoc />
        public RedirectEntity(bool retainQuery, bool permanent, bool force, EntityStrategy source, EntityStrategy target)
        {
            _retainQuery = retainQuery;
            _permanent = permanent;
            _force = force;
            _source = source;
            _target = target;
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

        public EntityStrategy Target
        {
            get => _target;
            set => SetPropertyValueAndDetectChanges(value, ref _target!, nameof(Target));
        }

        public EntityStrategy Source
        {
            get => _source;
            set => SetPropertyValueAndDetectChanges(value, ref _source!, nameof(Source));
        }

        public Guid SourceStrategy => Source.Strategy;

        public string SourceValue => Source.Value;

        public Guid TargetStrategy => Target.Strategy;

        public string TargetValue => Target.Value;

        private string GetDebuggerDisplay()
        {
            return $"From {Source.Value} to {Target.Value}";
        }
    }
}
