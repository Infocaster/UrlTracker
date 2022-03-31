using System.Collections.Generic;

namespace UrlTracker.Core.Intercepting.Models
{
    public interface IReadOnlyInterceptContext
    {
        T? Get<T>(string key);
        IReadOnlyCollection<string> Keys { get; }
    }

    public interface IInterceptContext
        : IReadOnlyInterceptContext
    {
        void Set(string key, object? value);
        int Count { get; }
    }
}