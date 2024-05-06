using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Processing.Filtering
{
    /// <summary>
    /// An implementation of <see cref="IClientErrorFilter" /> that ensures tracking only when the response is 404 NOT FOUND
    /// </summary>
    public class NotFoundClientErrorFilter : IClientErrorFilter
    {
        /// <inheritdoc/>
        public ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
            => new(EvaluateCandidate(context));

        private static bool EvaluateCandidate(HttpContext context)
        {
            return context.Response.StatusCode == 404;
        }
    }
}
