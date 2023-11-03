using System.Collections.Generic;
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
        IEnumerable<RecommendationResponse?> Update(IEnumerable<UpdateRequest> request);
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

            return _mapper.Map<RecommendationResponse>(recommendation)!;
        }

        /// <returns> Returns a collection of recommendations based on the provided request model values. </returns>
        public RecommendationCollectionResponse Get(ListRecommendationRequest request)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var result = _recommendationService.Get(request.Page, request.PageSize, orderingOptions: new RecommendationOrderingOptions()
            {
                OrderBy = request.OrderBy,
                Desc = request.Desc,
            });

            return _mapper.Map<RecommendationCollectionResponse>(result)!;
        }

        /// <returns> Returns the recommendation that was updated or null if the recommendation was not found</returns>
        public RecommendationResponse? Update(UpdateRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var recommendation = HandleUpdate(request);

            scope.Complete();
            return _mapper.Map<RecommendationResponse>(recommendation)!;
        }

        /// <returns> Returns a list of recommendation or null values that were updated. a null value is returned if the recommendation was not found</returns>
        public IEnumerable<RecommendationResponse?> Update(IEnumerable<UpdateRequest> request)
        {
            using var scope = _scopeProvider.CreateScope();

            var response = new List<RecommendationResponse?>();
            foreach (var item in request)
            {
                response.Add(_mapper.Map<RecommendationResponse>(HandleUpdate(item))!);
            }

            scope.Complete();
            return response;
        }

        private IRecommendation? HandleUpdate(UpdateRequest request)
        {
            var recommendation = _recommendationService.Get(request.Id);
            if (recommendation == null) return null;

            if (request.Ignore.HasValue)
            {
                recommendation.Ignore = request.Ignore.Value;
            }

            _recommendationService.Save(recommendation);
            return recommendation;
        }
    }
}
