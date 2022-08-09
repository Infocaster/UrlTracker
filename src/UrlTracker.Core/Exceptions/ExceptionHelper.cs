using System;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Exceptions
{
    internal static class ExceptionHelper
    {
        [ExcludeFromCodeCoverage]
        public static void WrapAsArgumentException(string parameter, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw new ArgumentException(null, parameter, e);
            }
        }
    }
}
