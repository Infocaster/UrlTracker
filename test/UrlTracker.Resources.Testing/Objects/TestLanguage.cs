using Moq;
using Umbraco.Core.Models;

namespace UrlTracker.Resources.Testing.Objects
{
    public static class TestLanguage
    {
        public static ILanguage Create(int id, string isoCode, string cultureName)
        {
            var languageMock = new Mock<ILanguage>();
            languageMock.Setup(x => x.Id).Returns(id);
            languageMock.Setup(x => x.IsoCode).Returns(isoCode);
            languageMock.Setup(x => x.CultureName).Returns(cultureName);
            return languageMock.Object;
        }
    }
}