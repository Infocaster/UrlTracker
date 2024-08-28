
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Base
{
    [DataContract]
    public class StrategyViewModel
        : IEquatable<StrategyViewModel>
    {
        [Required]
        [DataMember(Name = "strategy")]
        public Guid Strategy { get; set; }

        [Required]
        [DataMember(Name = "value")]
        public string Value { get; set; } = null!;

        public static StrategyViewModel FromEntity(EntityStrategy entity)
            => new () { Strategy = entity.Strategy, Value = entity.Value };

        public bool Equals(StrategyViewModel? other)
        {
            return other is not null &&
                (ReferenceEquals(this, other) ||
                (Strategy == other.Strategy
                && Value == other.Value));
        }

        public override bool Equals(object? obj)
        {
            return obj is StrategyViewModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Strategy, Value);
        }

        public static bool operator ==(StrategyViewModel? left, StrategyViewModel? right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(StrategyViewModel? left, StrategyViewModel? right)
        {
            return !(left == right);
        }
    }
}