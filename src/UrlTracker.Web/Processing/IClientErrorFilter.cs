using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Processing
{
    /// <summary>
    /// When implemented, this type provides means to decide if a client error response should be tracked or not
    /// </summary>
    public interface IClientErrorFilter
    {
        /// <summary>
        /// Analyze the current request and decide whether or not this request should be tracked as a client error
        /// </summary>
        /// <param name="context">The current request context</param>
        /// <returns><see langword="true"/> if the current request may be tracked, <see langword="false" /> otherwise</returns>
        /// <remarks>
        /// <para>
        /// NOTE: returning <see langword="true" /> does not guarantee that the current request will be tracked as results from other implementations are considered as well.
        /// Returning <see langword="false" /> does guarantee that the current request will not be tracked.
        /// </para>
        /// </remarks>
        ValueTask<bool> EvaluateCandidateAsync(HttpContext context);
    }
}
