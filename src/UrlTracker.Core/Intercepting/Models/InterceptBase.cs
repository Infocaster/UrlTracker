using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Intercepting.Models
{
    [ExcludeFromCodeCoverage]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class InterceptBase<T>
        : IIntercept<T>
    {
        public InterceptBase(T info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public T Info { get; }

        object IIntercept.Info => Info!;

        #region overrides
        public override string ToString()
        {
            return $"Intercept of: {Info}";
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
        #endregion
    }

    [ExcludeFromCodeCoverage]
    public class CachableInterceptBase<T>
        : InterceptBase<T>, ICachableIntercept<T>
    {
        public CachableInterceptBase(T info)
            : base(info)
        { }
    }

    public static class CachableInterceptBase
    {
        // create a static single instance for easy identification
        public static readonly ICachableIntercept NullIntercept = new CachableInterceptBase<object>(new object());
    }
}
