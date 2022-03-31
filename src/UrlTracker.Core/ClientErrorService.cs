using System;
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
    public class ClientErrorService
        : IClientErrorService
    {
        private readonly IClientErrorRepository _clientErrorRepository;
        private readonly IValidationHelper _validationHelper;
        private readonly IExceptionHelper _exceptionHelper;
        private readonly IUmbracoMapper _mapper;

        public ClientErrorService(IClientErrorRepository clientErrorRepository,
                                  IValidationHelper validationHelper,
                                  IExceptionHelper exceptionHelper,
                                  IUmbracoMapper mapper)
        {
            _clientErrorRepository = clientErrorRepository;
            _validationHelper = validationHelper;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
        }

        [ExcludeFromCodeCoverage]
        public Task<int> CountAsync(DateTime? start, DateTime? end)
        {
            return _clientErrorRepository.CountAsync(start ?? Defaults.Parameters.StartDate, end ?? Defaults.Parameters.EndDate);
        }

        public async Task<NotFound> AddAsync(NotFound notFound)
        {
            if (notFound is null) throw new ArgumentNullException(nameof(notFound));
            _exceptionHelper.WrapAsArgumentException(nameof(notFound), () =>
            {
                _validationHelper.EnsureValidObject(notFound);
            });

            var urlTrackerNotFound = _mapper.Map<UrlTrackerNotFound>(notFound);
            urlTrackerNotFound = await _clientErrorRepository.AddAsync(urlTrackerNotFound);
            return _mapper.Map<NotFound>(urlTrackerNotFound);
        }

        [ExcludeFromCodeCoverage]
        public async Task<RichNotFoundCollection> GetAsync(uint skip, uint take, string? query, OrderBy orderBy, bool descending)
        {
            var urlTrackerRichNotFounds = await _clientErrorRepository.GetAsync(skip, take, query, orderBy, descending);
            return _mapper.Map<RichNotFoundCollection>(urlTrackerRichNotFounds);
        }

        public Task DeleteAsync(string url, string? culture)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException($"'{nameof(url)}' cannot be null or whitespace.", nameof(url));
            }

            culture = culture.DefaultIfNullOrWhiteSpace(null);

            return _clientErrorRepository.DeleteAsync(url, culture);
        }

        [ExcludeFromCodeCoverage]
        public async Task<NotFound?> GetAsync(int id)
        {
            var result = await _clientErrorRepository.GetAsync(id);
            return _mapper.Map<NotFound>(result);
        }

        [ExcludeFromCodeCoverage]
        public Task UpdateAsync(NotFound notFound)
        {
            var entry = _mapper.Map<UrlTrackerNotFound>(notFound);
            return _clientErrorRepository.UpdateAsync(entry);
        }
    }
}
