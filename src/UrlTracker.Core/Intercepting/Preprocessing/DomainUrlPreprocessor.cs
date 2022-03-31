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
            var interceptingDomain = domains.FirstOrDefault(d => d.Url.Host!.Equals(url.Host) && url.Path!.StartsWith(d.Url.Path!));

            context.SetCulture(interceptingDomain?.LanguageIsoCode);
            context.SetRootNode(interceptingDomain?.NodeId);

            return new ValueTask<IInterceptContext>(context);
        }
    }
}
