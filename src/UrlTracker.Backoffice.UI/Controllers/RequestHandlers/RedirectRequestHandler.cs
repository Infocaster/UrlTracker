using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
using UrlTracker.Backoffice.UI.Notifications;
using UrlTracker.Core;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRedirectRequestHandler
    {
        RedirectResponse? Create(CreateRedirectRequest request);
        RedirectResponse? Delete(int id);
        void DeleteBulk(int[] ids);
        RedirectResponse? GetById(int id);
        IEnumerable<IRedirect> Get(int[] ids);
        Task<RedirectCollectionResponse> GetAsync(ListRedirectRequest request);
        RedirectResponse? Update(int id, RedirectRequest request);
        IEnumerable<RedirectResponse>? UpdateBulk(IEnumerable<RedirectBulkRequest> bulkRequest);
    }

    internal class RedirectRequestHandler : IRedirectRequestHandler
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IRecommendationService _recommendationService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IEventAggregator _eventAggregator;

        public RedirectRequestHandler(
            IRedirectRepository redirectRepository,
            IRecommendationService recommendationService,
            IScopeProvider scopeProvider,
            IEventAggregator eventAggregator)
        {
            _redirectRepository = redirectRepository;
            _recommendationService = recommendationService;
            _scopeProvider = scopeProvider;
            _eventAggregator = eventAggregator;
        }

        public async Task<RedirectCollectionResponse> GetAsync(ListRedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var filters = new RedirectFilters(
                request.Types?.Aggregate((l, r) => l | r) ?? RedirectType.All,
                request.SourceTypes);
            
            var entities = await _redirectRepository.GetAsync(request.Page * request.PageSize, request.PageSize, request.Query, filters, true);
            
            var result = RedirectCollectionResponse.FromEntityCollection(entities);
            Notify(result.Results);
            return result;
        }

        public IEnumerable<IRedirect> Get(int[] ids)
        {
            using var scope = _scopeProvider.CreateScope();

           return _redirectRepository.GetMany();
        }

        public RedirectResponse? GetById(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            var result = RedirectResponse.FromEntity(entity);
            Notify(result);
            return result;
        }

        public RedirectResponse? Create(CreateRedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = CreateEntity(request);
            _redirectRepository.Save(entity);

            if (request.SolvedRecommendation.HasValue)
            {
                var recommendation = _recommendationService.Get(request.SolvedRecommendation.Value);
                if (recommendation is null) return null;

                _recommendationService.Delete(recommendation);
            }

            scope.Complete();

            var result = RedirectResponse.FromEntity(entity);
            Notify(result);
            return result;
        }

        public RedirectResponse? Update(int id, RedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            HandleUpdate(entity, request);
            _redirectRepository.Save(entity);

            scope.Complete();

            var result = RedirectResponse.FromEntity(entity);
            Notify(result);
            return result;
        }

        public IEnumerable<RedirectResponse>? UpdateBulk(IEnumerable<RedirectBulkRequest> bulkRequest)
        {
            using var scope = _scopeProvider.CreateScope();

            var entities = _redirectRepository.GetMany(bulkRequest.Select(br => br.Id).ToArray());

            foreach (var request in bulkRequest)
            {
                var entity = entities.FirstOrDefault(e => e.Id == request.Id);
                if (entity is null) return null;

                HandleUpdate(entity, request.Redirect);
            }

            foreach (var entity in entities)
            {
                _redirectRepository.Save(entity);
            }

            scope.Complete();

            var result = bulkRequest
                .Select(br => entities.First(e => e.Id == br.Id))
                .Select(RedirectResponse.FromEntity)
                .ToList();
            Notify(result);
            return result;
        }

        public RedirectResponse? Delete(int id)
        {
            using var scope = _scopeProvider.CreateScope();
            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            _redirectRepository.Delete(entity);
            scope.Complete();

            var result = RedirectResponse.FromEntity(entity);
            Notify(result);
            return result;
        }

        public void DeleteBulk(int[] ids)
        {
            using var scope = _scopeProvider.CreateScope();
            _redirectRepository.DeleteBulk(ids);

            scope.Complete();
        }

        private void HandleUpdate(IRedirect entity, RedirectRequest request)
        {
            entity.Force = request.Force;
            entity.Permanent = request.Permanent;
            entity.RetainQuery = request.RetainQuery;
            entity.Source = CreateEntity(request.Source);
            entity.Target = CreateEntity(request.Target);
        }

        private static IRedirect CreateEntity(RedirectRequest request)
        {
            var entity = new RedirectEntity(
                        request.RetainQuery,
                        request.Permanent,
                        request.Force,
                        CreateEntity(request.Source),
                        CreateEntity(request.Target));
            if (request.Key.HasValue) entity.Key = request.Key.Value;

            return entity;
        }

        private static EntityStrategy CreateEntity(StrategyViewModel viewModel)
            => new(viewModel.Strategy, viewModel.Value);

        private void Notify(RedirectResponse redirect)
            => Notify(redirect.AsEnumerableOfOne());

        private void Notify(IEnumerable<RedirectResponse> redirects)
            => _eventAggregator.Publish(new ServingRedirectsNotification(redirects));
    }
}
