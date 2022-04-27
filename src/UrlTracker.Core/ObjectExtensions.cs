namespace UrlTracker.Core
{
    public static class ObjectExtensions
    {
        public static string DefaultIfNullOrWhiteSpace(this string input, string @default)
            => string.IsNullOrWhiteSpace(input) ? @default : input;
    }
}
