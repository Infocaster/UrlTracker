using Microsoft.Extensions.Options;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using UrlTracker.Web.Configuration;

namespace UrlTracker.Web.Tests.Configuration
{
    public class ReservedPathSettingsProviderTests
    {
        private IOptions<GlobalSettings>? _globalSettings;
        private ReservedPathSettingsProvider? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _globalSettings = Options.Create<GlobalSettings>(new GlobalSettings());
            _testSubject = new ReservedPathSettingsProvider(_globalSettings);
        }

        [TestCase(TestName = "Value returns reserved paths")]
        public void Value_NormalFlow_ReturnsReservedPaths()
        {
            // arrange
            _globalSettings!.Value.ReservedPaths = "/lorem,/ipsum";
            _globalSettings.Value.ReservedUrls = "/dolor,/sit";

            // act
            var result = _testSubject!.Paths;

            // assert
            Assert.That(result, Is.EquivalentTo(new[] { "lorem/", "ipsum/", "dolor/", "sit/" }));
        }
    }
}
