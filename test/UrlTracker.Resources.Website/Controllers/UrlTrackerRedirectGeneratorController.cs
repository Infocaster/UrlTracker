using Bogus;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Web.BackOffice.Controllers;
using UrlTracker.Core;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Resources.Website.Controllers
{
    public class UrlTrackerRedirectGeneratorController : UmbracoAuthorizedApiController
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IScopeProvider _scopeProvider;
        private static readonly Faker<IRedirect> redirectGenerator
            = new Faker<IRedirect>()
            .CustomInstantiator((f) => new RedirectEntity(f.Random.Bool(), f.Random.Bool(), f.Random.Bool(), f.GenerateSourceStrategy(), f.GenerateTargetStrategy()));

        public UrlTrackerRedirectGeneratorController(IRedirectRepository redirectRepository, IScopeProvider scopeProvider)
        {
            _redirectRepository = redirectRepository;
            _scopeProvider = scopeProvider;
        }

        [HttpPost]
        public IActionResult Generate()
        {
            using var scope = _scopeProvider.CreateScope();
            foreach(var redirect in redirectGenerator.Generate(100))
            {
                _redirectRepository.Save(redirect);
            }

            scope.Complete();
            return Ok();
        }

        [HttpPost]
        public IActionResult Clear()
        {
            using var scope = _scopeProvider.CreateScope();
            var all = _redirectRepository.GetMany();
            foreach (var redirect in all)
            {
                _redirectRepository.Delete(redirect);
            }

            scope.Complete();
            return Ok();
        }
    }
}
