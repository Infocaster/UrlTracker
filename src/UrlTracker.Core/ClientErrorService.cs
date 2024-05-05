using System;
using System.Collections.Generic;
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
    public interface IClientErrorService
    {
        Task<ClientError> AddAsync(ClientError ClientError);
        Task<ClientError?> GetAsync(string url);
        Task ReportAsync(string url, DateTime moment, string? referrer);
        Task<IEnumerable<ReferrerResponse>> GetClientErrorReferrersAsync(int id);
        Task<IEnumerable<DailyClientErrorResponse>> GetInRangeAsync(int id, DateTime start, DateTime end);
        Task CleanupAsync(DateTime upperDate);
    }

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
        public async Task<ClientError?> GetAsync(string url)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var entity = _clientErrorRepository
                .Get(scope.SqlContext.Query<IClientError>().Where(e => e.Url == url))
                .FirstOrDefault();

            if (entity is null) return null;

            var metaData = await _clientErrorRepository.GetMetaDataAsync(entity.Id);
            return new ClientError(entity, metaData.FirstOrDefault(md => md.ClientError == entity.Id));
        }

        public Task<IEnumerable<DailyClientErrorResponse>> GetInRangeAsync(int id, DateTime start, DateTime end)
        {

            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var entities = _clientErrorRepository.GetDailyClientErrorInRangeAsync(id, start, end);

            return entities;
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

        public async Task<IEnumerable<ReferrerResponse>> GetClientErrorReferrersAsync(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            var referrers = await _clientErrorRepository.GetReferrersByClientIdAsync(id);
            return referrers;
        }

        public async Task CleanupAsync(DateTime upperDate)
        {
            using var scope = _scopeProvider.CreateScope();

            await _clientErrorRepository.CleanupAsync(upperDate);

            scope.Complete();
        }
    }
}
