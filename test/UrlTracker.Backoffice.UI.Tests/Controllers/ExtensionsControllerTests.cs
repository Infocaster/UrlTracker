using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Backoffice.UI.Controllers;
using UrlTracker.Backoffice.UI.Controllers.Models.Extensions;
using UrlTracker.Backoffice.UI.Extensions;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Backoffice.UI.Tests.Controllers
{
    public class ExtensionsControllerTests : TestBase
    {
        private ExtensionsController _testSubject = null!;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new List<IMapDefinition>
            {
                new TestMapDefinition<IUrlTrackerDashboardPage, DashboardPagesResponsePage>
                {
                    To = new DashboardPagesResponsePage("test", UrlTracker.Backoffice.UI.Defaults.Routing.DashboardPageFolder + "test.html")
                }
            };
        }

        public override void SetUp()
        {
            UrlTrackerDashboardPageCollectionMock.Setup(obj => obj.Get()).Returns(new List<IUrlTrackerDashboardPage>
            {
                TestDashboardPage.Create("test", UrlTracker.Backoffice.UI.Defaults.Routing.DashboardPageFolder + "test.html")
            });
            _testSubject = new ExtensionsController(Mapper, UrlTrackerDashboardPageCollection);
        }

        [TestCase(TestName = "DashboardPages returns OK result")]
        public void DashboardPages_DefaultFlow_Returns200OK()
        {
            // arrange

            // act
            var result = _testSubject.DashboardPages();

            // assert
            Assert.That(result, Is.AssignableTo<OkObjectResult>());
            var objectResult = (OkObjectResult)result;
            Assert.That(objectResult.Value, Is.AssignableTo<DashboardPagesResponse>());
        }
    }
}
