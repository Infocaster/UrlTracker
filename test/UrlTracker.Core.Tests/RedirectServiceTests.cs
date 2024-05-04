using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class RedirectServiceTests
    {
        private Mock<IRedirectRepository> _redirectRepositoryMock;
        private UmbracoMapper _mapper;
        private Mock<IValidationHelper> _validationHelperMock;
        private ScopeProviderMock _scopeProviderMock;
        private RedirectService? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _redirectRepositoryMock = new Mock<IRedirectRepository>();
            _mapper = new UmbracoMapper(new MapDefinitionCollection(CreateMappers), Mock.Of<ICoreScopeProvider>(), new VoidLogger<UmbracoMapper>());
            _validationHelperMock = new Mock<IValidationHelper>();
            _scopeProviderMock = new ScopeProviderMock();
            _testSubject = new RedirectService(_redirectRepositoryMock.Object,
                                               _mapper,
                                               _validationHelperMock.Object,
                                               _scopeProviderMock.Provider);
        }

        private ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                TestMapDefinition.CreateTestMap<Core.Database.Entities.RedirectEntityCollection, RedirectCollection>(RedirectCollection.Create(Enumerable.Empty<Redirect>())),
                TestMapDefinition.CreateTestMap<Redirect, IRedirect>(new RedirectEntity(default, default, default, default!, default!)),
                TestMapDefinition.CreateTestMap<IRedirect, Redirect>(new Redirect())
            };
        }

        private Exception SetupValidationFails()
        {
            var exception = new Exception();
            _validationHelperMock!.Setup(obj => obj.EnsureValidObject(It.IsAny<Redirect>())).Throws(exception);
            return exception;
        }

        private void SetupValidationSuccessful()
        {
            _validationHelperMock!.Setup(obj => obj.EnsureValidObject(It.IsAny<Redirect>())).Verifiable();
        }

        private static void AssertArgumentNullException(AsyncTestDelegate code, string expectedParamName)
        {
            Assert.Multiple(() =>
            {
                var actualException = Assert.ThrowsAsync<ArgumentNullException>(code);
                Assert.That(actualException?.ParamName, Is.EqualTo(expectedParamName));
            });
        }

        private static void AssertValidationException(AsyncTestDelegate code, Exception expectedInnerException)
        {
            Assert.Multiple(() =>
            {
                var actualException = Assert.ThrowsAsync<ArgumentException>(code);
                Assert.That(actualException?.InnerException, Is.SameAs(expectedInnerException));
            });
        }

        private void AssertValidationNoExceptions(AsyncTestDelegate code)
        {
            Assert.DoesNotThrowAsync(code);
            _validationHelperMock!.Verify();
        }
    }
}
