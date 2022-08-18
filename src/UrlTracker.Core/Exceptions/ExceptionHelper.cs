using System;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Exceptions
{
    public static class ExceptionHelper
    {
        [ExcludeFromCodeCoverage]
        internal static void WrapAsArgumentException(string parameter, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Argument is invalid", parameter, e);
            }
        }
    }
}
