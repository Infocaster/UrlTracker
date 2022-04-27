using System;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class Domain
    {
        public Domain(int id, int? nodeId, string name, string languageIsoCode, Url url)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(languageIsoCode))
            {
                throw new ArgumentException($"'{nameof(languageIsoCode)}' cannot be null or whitespace.", nameof(languageIsoCode));
            }

            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            Id = id;
            NodeId = nodeId;
            Name = name;
            LanguageIsoCode = languageIsoCode;
            Url = url;
        }

        public int Id { get; }
        public int? NodeId { get; }
        public string Name { get; }
        public string LanguageIsoCode { get; }
        public Url Url { get; }
    }
}
