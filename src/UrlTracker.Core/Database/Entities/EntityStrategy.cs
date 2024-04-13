using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace UrlTracker.Core.Database.Entities
{
    /// <summary>
    /// A simplified model for strategies
    /// </summary>
    [DataContract]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    [ExcludeFromCodeCoverage]
    public class EntityStrategy : IEquatable<EntityStrategy>
    {
        /// <inheritdoc />
        public EntityStrategy(Guid strategy, string value)
        {
            Strategy = strategy;
            Value = value;
        }

        /// <summary>
        /// An identifier for the strategy
        /// </summary>
        [DataMember]
        public Guid Strategy { get; }

        /// <summary>
        /// A parameter to influence the behaviour of a strategy
        /// </summary>
        [DataMember]
        public string Value { get; }

        /// <summary>
        /// Create a new source strategy for static URL matching
        /// </summary>
        /// <param name="value">The strategy value</param>
        /// <returns>A new instance of <see cref="EntityStrategy"/></returns>
        public static EntityStrategy UrlSource(string value)
            => new(Defaults.DatabaseSchema.RedirectSourceStrategies.Url, value);

        /// <summary>
        /// Create a new source strategy for regular expression URL matching
        /// </summary>
        /// <param name="value">The strategy value</param>
        /// <returns>A new instance of <see cref="EntityStrategy"/></returns>
        public static EntityStrategy RegexSource(string value)
            => new(Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression, value);

        /// <summary>
        /// Create a new target strategy for static URL creation
        /// </summary>
        /// <param name="value">The strategy value</param>
        /// <returns>A new instance of <see cref="EntityStrategy"/></returns>
        public static EntityStrategy UrlTarget(string value)
            => new(Defaults.DatabaseSchema.RedirectTargetStrategies.Url, value);


        /// <summary>
        /// Create a new target strategy for content page URL creation
        /// </summary>
        /// <param name="value">The strategy value</param>
        /// <returns>A new instance of <see cref="EntityStrategy"/></returns>
        public static EntityStrategy ContentTarget(string value)
            => new(Defaults.DatabaseSchema.RedirectTargetStrategies.Content, value);

        /// <summary>
        /// Create a new target strategy for media URL creation
        /// </summary>
        /// <param name="value">The strategy value</param>
        /// <returns>A new instance of <see cref="EntityStrategy"/></returns>
        public static EntityStrategy MediaTarget(string value)
            => new(Defaults.DatabaseSchema.RedirectTargetStrategies.Media, value);

        /// <inheritdoc />
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is EntityStrategy entityStrategy && Equals(entityStrategy);
        }

        private string GetDebuggerDisplay()
        {
            return $"{Value} | {Strategy}";
        }

        /// <inheritdoc />
        public bool Equals(EntityStrategy? other)
        {
            return other is not null && Strategy == other.Strategy && Value == other.Value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Strategy, Value);
        }

        /// <inheritdoc />
        public static bool operator ==(EntityStrategy left, EntityStrategy right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public static bool operator !=(EntityStrategy left, EntityStrategy right)
        {
            return !left.Equals(right);
        }
    }
}
