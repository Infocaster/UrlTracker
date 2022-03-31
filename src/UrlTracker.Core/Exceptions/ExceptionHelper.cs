using System;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Exceptions
{
    public class ExceptionHelper
        : IExceptionHelper
    {
        [ExcludeFromCodeCoverage]
        public void WrapAsArgumentException(string parameter, Action action)
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
