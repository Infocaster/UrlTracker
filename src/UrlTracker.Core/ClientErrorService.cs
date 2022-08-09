using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
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
        private readonly IReferrerRepository _referrerRepository;
        private readonly IValidationHelper _validationHelper;
        private readonly IUmbracoMapper _mapper;
        private readonly IScopeProvider _scopeProvider;

        public ClientErrorService(IClientErrorRepository clientErrorRepository,
                                  IReferrerRepository referrerRepository,
                                  IValidationHelper validationHelper,
                                  IUmbracoMapper mapper,
                                  IScopeProvider scopeProvider)
        {
            _clientErrorRepository = clientErrorRepository;
            _referrerRepository = referrerRepository;
            _validationHelper = validationHelper;
            _mapper = mapper;
            _scopeProvider = scopeProvider;
        }

        [ExcludeFromCodeCoverage]
        public Task<int> CountAsync(DateTime? start, DateTime? end)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return _clientErrorRepository.CountAsync(start ?? Defaults.Parameters.StartDate, end ?? Defaults.Parameters.EndDate);
        }

        public Task<ClientError> AddAsync(ClientError clientError)
        {
            if (clientError is null) throw new ArgumentNullException(nameof(clientError));
            ExceptionHelper.WrapAsArgumentException(nameof(clientError), () =>
            {
                _validationHelper.EnsureValidObject(clientError);
            });

            var urlTrackerClientError = _mapper.Map<IClientError>(clientError)!;

            using var scope = _scopeProvider.CreateScope();
            _clientErrorRepository.Save(urlTrackerClientError);

            scope.Complete();
            return Task.FromResult(_mapper.Map<ClientError>(urlTrackerClientError)!);
        }

        [ExcludeFromCodeCoverage]
        public async Task<Models.ClientErrorCollection> GetAsync(uint skip, uint take, string? query, OrderBy orderBy, bool descending)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            var urlTrackerClientErrors = await _clientErrorRepository.GetAsync(skip, take, query, orderBy, descending);
            return _mapper.Map<Models.ClientErrorCollection>(urlTrackerClientErrors)!;
        }

        [ExcludeFromCodeCoverage]
        public Task DeleteAsync(ClientError ClientError)
        {
            var entity = _mapper.Map<IClientError>(ClientError)!;

            using var scope = _scopeProvider.CreateScope();
            _clientErrorRepository.Delete(entity);

            scope.Complete();
            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        public Task<ClientError?> GetAsync(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var result = _clientErrorRepository.Get(id);
            return Task.FromResult(_mapper.Map<ClientError>(result));
        }

        [ExcludeFromCodeCoverage]
        public Task UpdateAsync(ClientError clientError)
        {
            var entry = _mapper.Map<IClientError>(clientError)!;

            using var scope = _scopeProvider.CreateScope();
            _clientErrorRepository.Save(entry);

            scope.Complete();
            return Task.CompletedTask;
        }

        public Task<ClientError?> GetAsync(string url)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var entity = _clientErrorRepository.Get(scope.SqlContext.Query<IClientError>().Where(e => e.Url == url));

            return Task.FromResult<ClientError?>(_mapper.Map<ClientError>(entity.FirstOrDefault()));
        }

        public async Task ReportAsync(string url, DateTime moment, string? referrer)
        {
            using var scope = _scopeProvider.CreateScope();

            var clientError = (await _clientErrorRepository.GetAsync(new[] { url })).FirstOrDefault();
            if (clientError is null)
            {
                clientError = new ClientErrorEntity(url, false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound);
                _clientErrorRepository.Save(clientError);
            }

            if (clientError.Ignored) return;

            IReferrer? referrerEntity = null;
            if (referrer is not null)
            {
                referrerEntity = _referrerRepository.Get(referrer);
                if (referrerEntity is null)
                {
                    referrerEntity = new ReferrerEntity(referrer);
                    _referrerRepository.Save(referrerEntity);
                }
            }

            _clientErrorRepository.Report(clientError, moment, referrerEntity);

            scope.Complete();
        }
    }
}
