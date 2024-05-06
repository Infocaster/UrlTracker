using System.Threading.Tasks;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Processing
{
    /// <summary>
    /// When implemented, this type provides means to decide if a request should be intercepted or not
    /// </summary>
    public interface IRequestInterceptFilter
    {
        /// <summary>
        /// Analyze the url and decide whether or not this url should be intercepted
        /// </summary>
        /// <param name="url">The url of an incoming request</param>
        /// <returns><see langword="true"/> if the current request may be tracked, <see langword="false" /> otherwise</returns>
        /// <remarks>
        /// <para>
        /// NOTE: returning <see langword="true" /> does not guarantee that the current request will be intercepted as results from other implementations are considered as well.
        /// Returning <see langword="false" /> does guarantee that the current request will not be intercepted.
        /// </para>
        /// </remarks>
        ValueTask<bool> EvaluateCandidateAsync(Url url);
    }
}
