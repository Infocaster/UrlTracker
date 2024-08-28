using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Exceptions;
using UrlTracker.Core.Models;
using UrlTracker.Core.Validation;

namespace UrlTracker.Core
{
    public interface IRedirectService
    {
        Task<Redirect> AddAsync(Redirect redirect);
        Task DeleteAsync(Redirect redirect);
        Task<RedirectCollection> GetAsync();
        Task<Redirect?> GetAsync(int id);
        Task<Redirect> UpdateAsync(Redirect redirect);
    }

    public class RedirectService
        : IRedirectService
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IUmbracoMapper _mapper;
        private readonly IValidationHelper _validationHelper;
        private readonly IScopeProvider _scopeProvider;

        public RedirectService(IRedirectRepository redirectRepository,
                               IUmbracoMapper mapper,
                               IValidationHelper validationHelper,
                               IScopeProvider scopeProvider)
        {
            _redirectRepository = redirectRepository;
            _mapper = mapper;
            _validationHelper = validationHelper;
            _scopeProvider = scopeProvider;
        }

        [ExcludeFromCodeCoverage]
        public Task<RedirectCollection> GetAsync()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            var redirects = _redirectRepository.GetMany();
            return Task.FromResult(RedirectCollection.Create(_mapper.MapEnumerable<IRedirect, Redirect>(redirects)!));
        }

        [ExcludeFromCodeCoverage]
        public Task<Redirect?> GetAsync(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            var redirect = _redirectRepository.Get(id);
            return Task.FromResult(_mapper.Map<Redirect>(redirect));
        }

        public Task<Redirect> AddAsync(Redirect redirect)
        {
            using var scope = _scopeProvider.CreateScope();

            EnsureValidModel(redirect, nameof(redirect));

            var redirectEntity = _mapper.Map<IRedirect>(redirect)!;
            _redirectRepository.Save(redirectEntity);
            var result = _mapper.Map<Redirect>(redirectEntity)!;

            scope.Complete();
            return Task.FromResult(result);
        }

        public Task<Redirect> UpdateAsync(Redirect redirect)
        {
            using var scope = _scopeProvider.CreateScope();

            EnsureValidModel(redirect, nameof(redirect));
            ExceptionHelper.WrapAsArgumentException(nameof(redirect), () =>
            {
                if (!redirect.Id.HasValue) throw new ValidationException(new ValidationResult("This field is required", new[] { nameof(Redirect.Id) }), new RequiredAttribute(), redirect.Id);
            });

            var redirectEntity = _mapper.Map<IRedirect>(redirect)!;
            _redirectRepository.Save(redirectEntity);
            var result = _mapper.Map<Redirect>(redirectEntity)!;

            scope.Complete();
            return Task.FromResult(result);
        }

        [ExcludeFromCodeCoverage]
        public Task DeleteAsync(Redirect redirect)
        {
            using var scope = _scopeProvider.CreateScope();

            _redirectRepository.Delete(_mapper.Map<IRedirect>(redirect)!);
            scope.Complete();
            return Task.CompletedTask;
        }

        private void EnsureValidModel(object obj, string parameter)
        {
            if (obj is null) throw new ArgumentNullException(parameter);
            ExceptionHelper.WrapAsArgumentException(parameter, () =>
            {
                _validationHelper.EnsureValidObject(obj);
            });
        }
    }
}
