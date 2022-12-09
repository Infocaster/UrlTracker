using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Map
{
    internal class RecommendationMap : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<RecommendationEntityCollection, RecommendationCollectionResponse>(
                (source, context) => new RecommendationCollectionResponse(context.MapEnumerable<IRecommendation, RecommendationResponse>(source), source.Total));

            mapper.Define<IRecommendation, RecommendationResponse>(
                (source, context) => new RecommendationResponse(source.Id, source.Ignore, source.Url, source.Strategy.Key));
        }
    }
}
