using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UrlTracker.Core.Models
{
    /*
     * This simple url model is introduced to fix the shortcomings of the standard System.Uri object.
     * The purpose of this model is to make comparison between urls from different areas easier and more accurate.
     * 
     * How? By creating a single point of interpretation. This model interprets urls,
     *    so that we can more easily do partial comparisons by only comparing those parts that we're interested in.
     */

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Url
        : IEquatable<Url>
    {
        private IReadOnlyCollection<UrlType>? _availableUrlTypes;
        private Protocol? _protocol;
        private string? _host;
        private int? _port;
        private string? _query;
        private string? _path;
        private readonly object _availableUrlTypesLock = new();

        public Protocol? Protocol
        {
            get => _protocol; set
            {
                _availableUrlTypes = null;
                _protocol = value;
            }
        }
        public string? Host
        {
            get => _host; set
            {
                _availableUrlTypes = null;
                _host = value;
            }
        }
        public int? Port
        {
            get => _port; set
            {
                _availableUrlTypes = null;
                _port = value;
            }
        }
        public string? Path { get => _path; set => _path = value.DefaultIfNullOrWhiteSpace("/"); }
        public string? Query { get => _query; set => _query = value?.TrimStart('?').DefaultIfNullOrWhiteSpace(null); }
        public IReadOnlyCollection<UrlType> AvailableUrlTypes
        {
            get
            {
                // Use double if pattern with lock for best performance in a multithreaded environment
                if (_availableUrlTypes is null)
                {
                    lock (_availableUrlTypesLock)
                    {
                        if (_availableUrlTypes is null)
                        {
                            List<UrlType> result = new();
                            if (!string.IsNullOrWhiteSpace(Host) && Protocol is not null)
                            {
                                result.Add(UrlType.Absolute);
                            }
                            result.Add(UrlType.Relative);
                            _availableUrlTypes = result;
                        }
                    }
                }

                return _availableUrlTypes;
            }
        }

        #region Creation, parsing and casting
        // One may start to question the use of regex once it passes a certain threshold
        private static readonly Regex _urlPattern = new(@"^(((?<protocol>https?):\/\/)?(?<host>[a-z0-9-\.]+)(\:(?<port>\d+))?)?(?<path>\/([a-z0-9\-\._~!$&'\(\)\*\+,;=:@\/]|%[a-f0-9]{2})*)?(\?(?<query>([a-z0-9\-\._~!$'\(\)\*\+,;:@\/]|%[a-f0-9]{2})+\=([a-z0-9\-\._~!$'\(\)\*\+,;:@\/]|%[a-f0-9]{2})*(\&([a-z0-9\-\._~!$'\(\)\*\+,;:@\/]|%[a-f0-9]{2})+\=([a-z0-9\-\._~!$'\(\)\*\+,;:@\/]|%[a-f0-9]{2})*)*))?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        public static Url Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException($"'{nameof(input)}' cannot be null or whitespace.", nameof(input));
            }

            Match intercept = _urlPattern.Match(input);
            var protocolGroup = intercept.Groups["protocol"];
            var protocol = protocolGroup.Success ? (Protocol?)Enum.Parse(typeof(Protocol), protocolGroup.Value, true) : null;
            var hostGroup = intercept.Groups["host"];
            var host = hostGroup.Success ? hostGroup.Value : null;
            var portGroup = intercept.Groups["port"];
            var port = portGroup.Success ? (int?)int.Parse(portGroup.Value) : null;
            var pathGroup = intercept.Groups["path"];
            var path = pathGroup.Success ? pathGroup.Value : null;
            var queryGroup = intercept.Groups["query"];
            var query = queryGroup.Success ? queryGroup.Value : null;

            return new Url
            {
                Host = host,
                Port = port,
                Path = path,
                Protocol = protocol,
                Query = query,
            };
        }

        [ExcludeFromCodeCoverage]
        public static Url Create(Protocol? protocol, string? host, int? port, string? path, string? query)
        {
            return new Url
            {
                Protocol = protocol,
                Host = host,
                Port = port,
                Path = path,
                Query = query
            };
        }

        public static Url FromAbsoluteUri(Uri uri)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!uri.IsAbsoluteUri) throw new ArgumentException(null, nameof(uri));

            var result = new Url
            {
                Path = '/' + uri.AbsolutePath.Trim('/'),
                Protocol = Enum.TryParse(uri.Scheme, true, out Protocol protocolValue) ? protocolValue : null,
                Host = uri.Host,
                Port = uri.Port != -1 && !uri.IsDefaultPort ? uri.Port : null,
                Query = uri.Query
            };

            return result;
        }
        #endregion

        #region To String
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return ToString(AvailableUrlTypes.First());
        }

        public string ToString(UrlType urlType, bool ensureTrailingSlash = false, bool excludeQuery = false)
        {
            if (!AvailableUrlTypes.Contains(urlType)) throw new ArgumentOutOfRangeException($"This url cannot be converted to a string with url type '{urlType}'");
            StringBuilder sb = new();
            if (urlType == UrlType.Absolute)
            {
                sb.Append(Protocol.ToString()!.ToLowerInvariant() + "://");
                sb.Append(Host);
                if (Port.HasValue)
                {
                    sb.Append(":" + Port);
                }
            }
            var path = Path!.TrimEnd('/');
            if (ensureTrailingSlash) path += "/";
            sb.Append(path);

            if (!excludeQuery && Query is not null)
            {
                sb.Append("?" + Query);
            }
            return sb.ToString();
        }
        #endregion

        #region operators & comparison
        public bool Equals(Url? other)
        {
            return other is not null
                && other.Protocol == Protocol
                && other.Host == Host
                && other.Port == Port
                && other.Path == Path
                && other.Query == Query;
        }

        public bool ExtrapolatesTo(Url? other)
        {
            // The protocol only matters if the domain has a protocol specified
            // The host only matters if the domain has a host specified
            // The port only matters if the domain has either a host or a port specified
            // The path of the domain must be contained in the incoming url path
            if (other is null) return false;
            var iHaveNoHost = string.IsNullOrWhiteSpace(Host);
            return (!Protocol.HasValue || Protocol == other.Protocol) &&
                   (iHaveNoHost || Host!.Equals(other.Host)) &&
                   (iHaveNoHost || Port == other.Port) &&
                   other.Path!.StartsWith(Path!);
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object? obj)
        {
            return obj is Url other && Equals(other);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return HashCode.Combine(Protocol, Host, Port, Path, Query);
        }

        [ExcludeFromCodeCoverage]
        public static bool operator ==(Url first, Url second)
        {
            return first.Equals(second);
        }

        [ExcludeFromCodeCoverage]
        public static bool operator !=(Url first, Url second)
        {
            return !first.Equals(second);
        }
        #endregion


        [ExcludeFromCodeCoverage]
        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }

    public enum Protocol
    {
        Http,
        Https
    }

    public enum UrlType
    {
        Absolute,
        Relative
    }
}
