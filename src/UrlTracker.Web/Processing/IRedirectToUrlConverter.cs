using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Processing
{
    /// <summary>
    /// When implemented, this type provides functionality to convert redirect models to URL strings
    /// </summary>
    public interface IRedirectToUrlConverter
    {
        /// <summary>
        /// When implemented, this method indicates if this converter can convert a given redirect to a URL string
        /// </summary>
        /// <param name="redirect">The redirect to convert</param>
        /// <returns><see langword="true"/> if this converter can convert the given redirect, <see langword="false"/> otherwise</returns>
        bool CanHandle(Redirect redirect);

        /// <summary>
        /// When implemented, this method converts a given redirect to a URL string
        /// </summary>
        /// <param name="redirect">The redirect to convert</param>
        /// <param name="context">The context in which the redirect should be converted</param>
        /// <returns>The redirect URL as a <see langword="string"/></returns>
        string? Handle(Redirect redirect, HttpContext context);
    }
}