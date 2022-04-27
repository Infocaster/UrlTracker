using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core
{
    public static class IRedirectContextExtensions
    {
        private const string _cultureKey = "icUrlTrackerCulture";
        private const string _rootNodeKey = "icUrlTrackerRootNode";

        public static void SetCulture(this IInterceptContext context, string culture)
            => context.Set(_cultureKey, culture);

        public static string GetCulture(this IReadOnlyInterceptContext context)
            => context.Get<string>(_cultureKey);

        public static void SetRootNode(this IInterceptContext context, int? rootNodeId)
            => context.Set(_rootNodeKey, rootNodeId);

        public static int? GetRootNode(this IReadOnlyInterceptContext context)
            => context.Get<int?>(_rootNodeKey);
    }
}
