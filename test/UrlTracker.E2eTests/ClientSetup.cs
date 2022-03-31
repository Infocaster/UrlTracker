using NUnit.Framework;
using UrlTracker.Resources.Testing.Clients;

namespace UrlTracker.E2eTests
{
    [SetUpFixture]
    public class ClientSetup
    {
        private static WebsiteClient? _websiteClient;
        private static readonly object _websiteClientLock = new();

        public static WebsiteClient WebsiteClient
        {
            get
            {
                if(_websiteClient is null)
                {
                    lock (_websiteClientLock)
                    {
                        if(_websiteClient is null)
                        {
                            _websiteClient = WebsiteClient.Create("http://urltracker.ic");
                        }
                    }
                }

                return _websiteClient;
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _websiteClient?.Dispose();
        }
    }
}
