using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Core;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRecommendationRequestHandler
    {
        RecommendationCollectionResponse Get(ListRecommendationRequest request);
    }

    internal class RecommendationRequestHandler : IRecommendationRequestHandler
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IUmbracoMapper _mapper;

        public RecommendationRequestHandler(IRecommendationService recommendationService, IUmbracoMapper mapper)
        {
            _recommendationService = recommendationService;
            _mapper = mapper;
        }

        public RecommendationCollectionResponse Get(ListRecommendationRequest request)
        {
            var result = _recommendationService.Get(request.Page, request.PageSize);
            return _mapper.Map<RecommendationCollectionResponse>(result)!;
        }
    }
}
