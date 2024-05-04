using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;
using UrlTracker.Core.Validation;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests
{
    public partial class ClientErrorServiceTests
    {
        private IUmbracoMapper _mapper;
        private ScopeProviderMock _scopeProviderMock;
        private ClientErrorService _testSubject = null!;
        private Mock<IClientErrorRepository> _clientErrorRepositoryMock;
        private Mock<IReferrerRepository> _referrerRepositoryMock;
        private Mock<IValidationHelper> _validationHelperMock;

        [SetUp]
        public void SetUp()
        {
            _clientErrorRepositoryMock = new Mock<IClientErrorRepository>();
            _referrerRepositoryMock = new Mock<IReferrerRepository>();
            _validationHelperMock = new Mock<IValidationHelper>();
            _mapper = new UmbracoMapper(new MapDefinitionCollection(CreateMappers), Mock.Of<ICoreScopeProvider>(), new VoidLogger<UmbracoMapper>());
            _scopeProviderMock = new ScopeProviderMock();
            _testSubject = new ClientErrorService(_clientErrorRepositoryMock.Object, _referrerRepositoryMock.Object, _validationHelperMock.Object, _mapper, _scopeProviderMock.Provider);
        }

        private ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                TestMapDefinition.CreateTestMap<ClientError, IClientError>(new ClientErrorEntity("http://example.com", false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound)),
                TestMapDefinition.CreateTestMap<IClientError, ClientError>(new ClientError("http://example.com"))
            };
        }
    }
}
