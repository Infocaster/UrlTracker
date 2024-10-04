namespace UrlTracker.Core
{
    public static class StringExtensions
    {
        // Introducing a method to mitigate what seems to be an umbraco bug.
        //    Umbraco returns culture codes in lowercase (for example: 'nl-nl')
        //    IContent accepts lowercase, IPublishedContent requires upper case characters (for example: 'nl-NL')
        public static string? NormalizeCulture(this string? input)
        {
            if (input is null) return null;
            int index = input.IndexOf('-') + 1;
            if (index == 0) return input;
            return input[..index] + input[index..].ToUpperInvariant();
        }
    }
}
