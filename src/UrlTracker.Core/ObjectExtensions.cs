using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns a default value if the input string is either null, empty or whitespace
        /// </summary>
        /// <param name="input">The string that should not be null or whitespace</param>
        /// <param name="default">The default value to return if the input string is null or whitespace</param>
        /// <returns>Input if not null or whitespace, default otherwise</returns>
        [return: NotNullIfNotNull(nameof(@default))]
        public static string? DefaultIfNullOrWhiteSpace(this string? input, string? @default)
            => string.IsNullOrWhiteSpace(input) ? @default : input;
    }
}
