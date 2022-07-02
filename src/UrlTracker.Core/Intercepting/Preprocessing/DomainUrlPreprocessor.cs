using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Preprocessing
{
    public class DomainUrlPreprocessor
        : IInterceptPreprocessor
    {
        private readonly IDomainProvider _domainProvider;

        public DomainUrlPreprocessor(IDomainProvider domainProvider)
        {
            _domainProvider = domainProvider;
        }

        public ValueTask<IInterceptContext> PreprocessUrlAsync(Url url, IInterceptContext context)
        {
            var domains = _domainProvider.GetDomains();
            var interceptingDomain = domains.FirstOrDefault(d =>
            {
                // The protocol only matters if the domain has a protocol specified
                // The host only matters if the domain has a host specified
                // The port only matters if the domain has either a host or a port specified
                // The path of the domain must be contained in the incoming url path
                bool hasNoHost = string.IsNullOrWhiteSpace(d.Url.Host);
                return (!d.Url.Protocol.HasValue || d.Url.Protocol == url.Protocol) &&
                       (hasNoHost || d.Url.Host!.Equals(url.Host)) &&
                       (hasNoHost || d.Url.Port == url.Port) &&
                       url.Path!.StartsWith(d.Url.Path!);
            });

            context.SetCulture(interceptingDomain?.LanguageIsoCode);
            context.SetRootNode(interceptingDomain?.NodeId);

            return new ValueTask<IInterceptContext>(context);
        }
    }
}
