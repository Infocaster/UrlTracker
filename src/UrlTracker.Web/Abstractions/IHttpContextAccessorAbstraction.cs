using System.Web;

namespace UrlTracker.Web.Abstractions
{
    public interface IHttpContextAccessorAbstraction
    {
        HttpContextBase HttpContext { get; }
    }
}