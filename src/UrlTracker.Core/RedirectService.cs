using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Exceptions;
using UrlTracker.Core.Models;
using UrlTracker.Core.Validation;

namespace UrlTracker.Core
{
    public class RedirectService
        : IRedirectService
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IUmbracoMapper _mapper;
        private readonly IValidationHelper _validationHelper;
        private readonly IExceptionHelper _exceptionHelper;

        public RedirectService(IRedirectRepository redirectRepository,
                               IUmbracoMapper mapper,
                               IValidationHelper validationHelper,
                               IExceptionHelper exceptionHelper)
        {
            _redirectRepository = redirectRepository;
            _mapper = mapper;
            _validationHelper = validationHelper;
            _exceptionHelper = exceptionHelper;
        }

        [ExcludeFromCodeCoverage]
        public async Task<RedirectCollection> GetAsync()
        {
            var redirects = await _redirectRepository.GetAsync().ConfigureAwait(false);
            return _mapper.Map<RedirectCollection>(redirects)!;
        }

        [ExcludeFromCodeCoverage]
        public async Task<RedirectCollection> GetAsync(uint skip, uint take, string? query = null, OrderBy order = OrderBy.Created, bool descending = true)
        {
            var redirects = await _redirectRepository.GetAsync(skip, take, query, order, descending).ConfigureAwait(false);
            return _mapper.Map<RedirectCollection>(redirects)!;
        }

        public async Task<Redirect> AddAsync(Redirect redirect)
        {
            EnsureValidModel(redirect, nameof(redirect));

            var urlTrackerRedirect = _mapper.Map<UrlTrackerRedirect>(redirect)!;
            urlTrackerRedirect = await _redirectRepository.AddAsync(urlTrackerRedirect);
            var result = _mapper.Map<Redirect>(urlTrackerRedirect)!;

            return result;
        }

        public async Task<Redirect> UpdateAsync(Redirect redirect)
        {
            EnsureValidModel(redirect, nameof(redirect));
            _exceptionHelper.WrapAsArgumentException(nameof(redirect), () =>
            {
                if (!redirect.Id.HasValue) throw new ValidationException(new ValidationResult("This field is required", new[] { nameof(Redirect.Id) }), new RequiredAttribute(), redirect.Id);
            });

            var urlTrackerRedirect = _mapper.Map<UrlTrackerRedirect>(redirect)!;
            urlTrackerRedirect = await _redirectRepository.UpdateAsync(urlTrackerRedirect);
            var result = _mapper.Map<Redirect>(urlTrackerRedirect)!;

            return result;
        }

        private void EnsureValidModel(object obj, string parameter)
        {
            if (obj is null) throw new ArgumentNullException(parameter);
            _exceptionHelper.WrapAsArgumentException(parameter, () =>
            {
                _validationHelper.EnsureValidObject(obj);
            });
        }
    }
}
