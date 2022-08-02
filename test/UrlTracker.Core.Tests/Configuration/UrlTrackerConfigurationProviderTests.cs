using NUnit.Framework;
using UrlTracker.Core.Configuration;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests.Configuration
{
    public class UrlTrackerConfigurationProviderTests : TestBase
    {
        private UrlTrackerConfigurationProvider _testSubject;

        public override void SetUp()
        {
            _testSubject = new UrlTrackerConfigurationProvider(AppSettingsAbstraction);
        }

        [TestCase(TestName = "Value returns correct settings")]
        public void Value_NormalFlow_ReturnsCorrectSettings()
        {
            // arrange
            AppSettingsAbstractionMock.Setup(obj => obj.Get("urlTracker:disabled")).Returns("true");
            AppSettingsAbstractionMock.Setup(obj => obj.Get("urlTracker:trackingDisabled")).Returns("true");
            AppSettingsAbstractionMock.Setup(obj => obj.Get("urlTracker:notFoundTrackingDisabled")).Returns("true");
            AppSettingsAbstractionMock.Setup(obj => obj.Get("urlTracker:enableLogging")).Returns("true");
            AppSettingsAbstractionMock.Setup(obj => obj.Get("urlTracker:appendPortNumber")).Returns("true");
            AppSettingsAbstractionMock.Setup(obj => obj.Get("urlTracker:hasDomainOnChildNode")).Returns("true");
            AppSettingsAbstractionMock.Setup(obj => obj.Get("urlTracker:blockedUrlsList")).Returns("testa, testb");

            // act
            var result = _testSubject.Value;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.AppendPortNumber, Is.True);
                Assert.That(result.HasDomainOnChildNode, Is.True);
                Assert.That(result.IsDisabled, Is.True);
                Assert.That(result.IsNotFoundTrackingDisabled, Is.True);
                Assert.That(result.IsTrackingDisabled, Is.True);
                Assert.That(result.LoggingEnabled, Is.True);
                Assert.That(result.BlockedUrlsList, Is.Not.Null);
            });
        }

        [TestCase(TestName = "Value returns default values when no values are configured")]
        public void Value_NoValueForConfigurations_ReturnsDefaults()
        {
            // arrange (nothing to arrange here)

            // act
            var result = _testSubject.Value;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.AppendPortNumber, Is.False);
                Assert.That(result.HasDomainOnChildNode, Is.False);
                Assert.That(result.IsDisabled, Is.False);
                Assert.That(result.IsNotFoundTrackingDisabled, Is.False);
                Assert.That(result.IsTrackingDisabled, Is.False);
                Assert.That(result.LoggingEnabled, Is.False);
                Assert.That(result.BlockedUrlsList, Is.Empty);
            });
        }
    }
}
