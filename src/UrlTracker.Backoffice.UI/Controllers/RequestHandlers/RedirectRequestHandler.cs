using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
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

        public RedirectRequestHandler(
            IRedirectRepository redirectRepository,
            IRecommendationService recommendationService,
            IScopeProvider scopeProvider)
        {
            _redirectRepository = redirectRepository;
            _recommendationService = recommendationService;
            _scopeProvider = scopeProvider;
        }

        public async Task<RedirectCollectionResponse> GetAsync(ListRedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var combinedType = request.Types?.Aggregate((l, r) => l | r) ?? RedirectType.All;
            
            var entities = await _redirectRepository.GetAsync(request.Page * request.PageSize, request.PageSize, request.Query, combinedType, true);
            return RedirectCollectionResponse.FromEntityCollection(entities);
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

            return RedirectResponse.FromEntity(entity);
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
            return RedirectResponse.FromEntity(entity);
        }

        public RedirectResponse? Update(int id, RedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            HandleUpdate(entity, request);
            _redirectRepository.Save(entity);

            scope.Complete();
            return RedirectResponse.FromEntity(entity);
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
            return bulkRequest
                .Select(br => entities.First(e => e.Id == br.Id))
                .Select(RedirectResponse.FromEntity);
        }

        public RedirectResponse? Delete(int id)
        {
            using var scope = _scopeProvider.CreateScope();
            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            _redirectRepository.Delete(entity);
            scope.Complete();

            return RedirectResponse.FromEntity(entity);
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

    }
}
