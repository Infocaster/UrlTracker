using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Core;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;
using static Umbraco.Cms.Core.Constants.HttpContext;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRecommendationRequestHandler
    {
        RecommendationCollectionResponse Get(ListRecommendationRequest request);
        RecommendationResponse? Update(UpdateRequest request);
        RecommendationResponse? Delete(DeleteRequest request);
        IEnumerable<RecommendationResponse>? Update(IEnumerable<UpdateRequest> request);
    }

    internal class RecommendationRequestHandler : IRecommendationRequestHandler
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IUmbracoMapper _mapper;
        private readonly IScopeProvider _scopeProvider;

        public RecommendationRequestHandler(IRecommendationService recommendationService, IUmbracoMapper mapper, IScopeProvider scopeProvider)
        {
            _recommendationService = recommendationService;
            _mapper = mapper;
            _scopeProvider = scopeProvider;
        }

        /// <returns> Returns the recommendation that was deleted or null if the recommendation was not found</returns>
        public RecommendationResponse? Delete(DeleteRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var recommendation = _recommendationService.Get(request.Id);
            if (recommendation == null) return null;

            _recommendationService.Delete(recommendation);
            scope.Complete();

            return RecommendationResponse.FromEntity(recommendation);
        }

        /// <returns> Returns a collection of recommendations based on the provided request model values. </returns>
        public RecommendationCollectionResponse Get(ListRecommendationRequest request)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var result = _recommendationService.Get(
                request.Page,
                request.PageSize,
                new RecommendationOrderingOptions()
                {
                    OrderBy = request.OrderBy,
                    Desc = request.Desc,
                },
                new RecommendationFilterOptions
                {
                    Query = request.Query,
                    Types = request.Types
                });

            return RecommendationCollectionResponse.FromEntityCollection(result);
        }

        /// <returns> Returns the recommendation that was updated or null if the recommendation was not found</returns>
        public RecommendationResponse? Update(UpdateRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = _recommendationService.Get(request.Id);
            if (entity is null) return null;

            HandleUpdate(entity, request);

            _recommendationService.Save(entity);

            scope.Complete();
            return RecommendationResponse.FromEntity(entity);
        }

        /// <returns> Returns a list of recommendation or null values that were updated. a null value is returned if the recommendation was not found</returns>
        public IEnumerable<RecommendationResponse>? Update(IEnumerable<UpdateRequest> request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entities = _recommendationService.GetMany(request.Select(r => r.Id).ToArray());

            foreach (var item in request)
            {
                var entity = entities.FirstOrDefault(e => e.Id == item.Id);
                if (entity is null) return null;

                HandleUpdate(entity, item);
            }

            foreach (var entity in entities)
            {
                _recommendationService.Save(entity);
            }
            scope.Complete();

            return request
                .Select(r => entities.First(e => e.Id == r.Id))
                .Select(RecommendationResponse.FromEntity);
        }

        private void HandleUpdate(IRecommendation entity, UpdateRequest request)
        {
            if (request.Ignore.HasValue)
            {
                entity.Ignore = request.Ignore.Value;
            }
        }
    }
}
