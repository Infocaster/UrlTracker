using System;

namespace UrlTracker.Core.Exceptions
{
    public interface IExceptionHelper
    {
        void WrapAsArgumentException(string parameter, Action action);
    }
}