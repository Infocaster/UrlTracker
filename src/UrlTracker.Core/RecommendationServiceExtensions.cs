using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core
{
    public static class RecommendationServiceExtensions
    {
        public static IRecommendation CreateAndSave(this IRecommendationService service, string url, IRedactionScore score)
        {
            var entity = service.Create(url, score);
            service.Save(entity);

            return entity;
        }

        public static IRecommendation CreateAndSave(this IRecommendationService service, string url, Guid scoreKey)
        {
            var entity = service.Create(url, scoreKey);
            service.Save(entity);

            return entity;
        }

        public static IRecommendation GetOrCreate(this IRecommendationService service, string url, IRedactionScore score)
        {
            return service.Get(url, score) ?? service.Create(url, score);
        }
    }
}
