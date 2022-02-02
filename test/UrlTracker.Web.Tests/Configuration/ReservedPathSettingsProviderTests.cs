using NUnit.Framework;
using UrlTracker.Resources.Testing;
using UrlTracker.Web.Configuration;

namespace UrlTracker.Web.Tests.Configuration
{
    public class ReservedPathSettingsProviderTests : TestBase
    {
        private ReservedPathSettingsProvider _testSubject;

        public override void SetUp()
        {
            _testSubject = new ReservedPathSettingsProvider(GlobalSettings);
        }

        [TestCase(TestName = "Value returns reserved paths")]
        public void Value_NormalFlow_ReturnsReservedPaths()
        {
            // arrange
            GlobalSettingsMock.SetupGet(obj => obj.ReservedPaths).Returns("/lorem,/ipsum");
            GlobalSettingsMock.SetupGet(obj => obj.ReservedUrls).Returns("/dolor,/sit");

            // act
            var result = _testSubject.Value;

            // assert
            CollectionAssert.AreEquivalent(new[] { "lorem/", "ipsum/", "dolor/", "sit/" }, result.Paths);
        }
    }
}
