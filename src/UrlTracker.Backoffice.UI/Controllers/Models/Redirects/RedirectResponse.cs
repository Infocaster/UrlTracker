using System;
using System.Runtime.Serialization;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    [DataContract]
    internal class RedirectResponse
        : RedirectViewModelBase, IEquatable<RedirectResponse>
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "createDate")]
        public DateTime CreateDate { get; set; }

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
                && Force == other.Force));
        }

        public override bool Equals(object? obj)
        {
            return obj is RedirectResponse other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Key, CreateDate, Source, Target, Permanent, RetainQuery, Force);
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
