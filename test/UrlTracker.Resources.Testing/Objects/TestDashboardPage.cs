using Moq;
using UrlTracker.Backoffice.UI.Extensions;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestDashboardPage
    {
        public static IUrlTrackerDashboardPage Create(string alias, string view)
        {
            var result = new Mock<IUrlTrackerDashboardPage>();
            result.SetupGet(x => x.Alias).Returns(alias);
            result.SetupGet(x => x.View).Returns(view);
            return result.Object;
        }
    }
}
