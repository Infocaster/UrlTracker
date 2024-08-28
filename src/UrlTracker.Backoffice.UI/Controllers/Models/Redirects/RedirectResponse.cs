using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    [DataContract]
    public class RedirectResponse
        : RedirectViewModelBase, IEquatable<RedirectResponse>
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        [DataMember(Name = "createDate")]
        public DateTime CreateDate { get; set; }

        [DataMember(Name = "updateDate")]
        public DateTime UpdateDate { get; set; }

        [DataMember(Name = "additionalData")]
        public IDictionary<string, object?> AdditionalData { get; } = new Dictionary<string, object?>();

        public static RedirectResponse FromEntity(IRedirect entity)
            => new()
            {
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate == default ? entity.CreateDate : entity.UpdateDate,
                Force = entity.Force,
                Advanced = entity.Advanced,
                Id = entity.Id,
                Key = entity.Key,
                Permanent = entity.Permanent,
                RetainQuery = entity.RetainQuery,
                Source = StrategyViewModel.FromEntity(entity.Source),
                Target = StrategyViewModel.FromEntity(entity.Target)
            };

        public bool Equals(RedirectResponse? other)
        {
            return other is not null &&
                (ReferenceEquals(this, other) ||
                  (Id == other.Id
                && Key == other.Key
                && CreateDate == other.CreateDate
                && Source == other.Source
                && Target == other.Target
                && Permanent == other.Permanent
                && RetainQuery == other.RetainQuery
                && Force == other.Force
                && Advanced == other.Advanced));
        }

        public override bool Equals(object? obj)
        {
            return obj is RedirectResponse other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Id);
            hash.Add(Key);
            hash.Add(CreateDate);
            hash.Add(Source);
            hash.Add(Target);
            hash.Add(Permanent);
            hash.Add(RetainQuery);
            hash.Add(Force);
            hash.Add(Advanced);
            return hash.ToHashCode();
        }

        public static bool operator ==(RedirectResponse? left, RedirectResponse? right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(RedirectResponse? left, RedirectResponse? right)
        {
            return !(left == right);
        }
    }
}
