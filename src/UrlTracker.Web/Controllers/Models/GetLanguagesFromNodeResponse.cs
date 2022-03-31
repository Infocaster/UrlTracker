using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Web.Controllers.Models
{
    [ExcludeFromCodeCoverage]
    public class GetLanguagesFromNodeResponseLanguage
    {
        public GetLanguagesFromNodeResponseLanguage(string isoCode, string cultureName)
        {
            IsoCode = isoCode;
            CultureName = cultureName;
        }

        public int Id { get; set; }
        public string IsoCode { get; set; }
        public string CultureName { get; set; }
    }
}
