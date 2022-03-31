using System.Collections.Generic;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Exceptions;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests
{
    public partial class ClientErrorServiceTests : TestBase
    {
        private ClientErrorService? _testSubject;

        public override void SetUp()
        {
            _testSubject = new ClientErrorService(ClientErrorRepository, ValidationHelper, new ExceptionHelper(), Mapper!);
        }

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                CreateTestMap<NotFound, UrlTrackerNotFound>(new UrlTrackerNotFound("http://example.com")),
                CreateTestMap<UrlTrackerNotFound, NotFound>(new NotFound("http://example.com"))
            };
        }
    }
}
