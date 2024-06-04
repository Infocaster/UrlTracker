using Asp.Versioning;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Resources.Website.Controllers
{
    [ApiController]
    [ApiVersion(Defaults.Routing.V1.ApiVersion)]
    [MapToApi(Defaults.Routing.V1.ApiName)]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [Route(Defaults.Routing.V1.Route)]
    public class UrlTrackerRedirectGeneratorController : Controller
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
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
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
        [MapToApiVersion(Defaults.Routing.V1.ApiVersion)]
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
