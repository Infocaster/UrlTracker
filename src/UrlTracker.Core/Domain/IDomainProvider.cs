using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Domain
{
    public interface IDomainProvider
    {
        DomainCollection GetDomains();
        DomainCollection GetDomains(int nodeId);
    }
}