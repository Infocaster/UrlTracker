using System.Collections.Generic;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests
{
    public partial class ClientErrorServiceTests : TestBase
    {
        private ClientErrorService _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new ClientErrorService(ClientErrorRepository, ReferrerRepository, ValidationHelper, Mapper, ScopeProviderMock.Provider);
        }

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                CreateTestMap<ClientError, IClientError>(new ClientErrorEntity("http://example.com", false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound)),
                CreateTestMap<IClientError, ClientError>(new ClientError("http://example.com"))
            };
        }
    }
}
