﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database.Models.Entities;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests
{
    public partial class RedirectServiceTests : TestBase
    {
        private RedirectService? _testSubject;

        public override void SetUp()
        {
            _testSubject = new RedirectService(RedirectRepository,
                                               Mapper!,
                                               ValidationHelper,
                                               ScopeProviderMock!.Provider);
        }

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                CreateTestMap<Core.Database.Entities.RedirectEntityCollection, RedirectCollection>(RedirectCollection.Create(Enumerable.Empty<Redirect>())),
                CreateTestMap<Redirect, IRedirect>(new RedirectEntity(default, default, default, default, default, default, default, default, default, default)),
                CreateTestMap<IRedirect, Redirect>(new Redirect())
            };
        }

        private Exception SetupValidationFails()
        {
            var exception = new Exception();
            ValidationHelperMock!.Setup(obj => obj.EnsureValidObject(It.IsAny<Redirect>())).Throws(exception);
            return exception;
        }

        private void SetupValidationSuccessful()
        {
            ValidationHelperMock!.Setup(obj => obj.EnsureValidObject(It.IsAny<Redirect>())).Verifiable();
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
            ValidationHelperMock!.Verify();
        }
    }
}
