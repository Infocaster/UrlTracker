using NUnit.Framework;
using UrlTracker.Resources.Testing.Clients;

namespace UrlTracker.E2eTests
{
    [SetUpFixture]
    public class ClientSetup
    {
        public static WebsiteClient WebsiteClient;

        [OneTimeSetUp]
        public void SetUp()
        {
            WebsiteClient = WebsiteClient.Create("http://urltracker.ic");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            WebsiteClient?.Dispose();
        }
    }
}
