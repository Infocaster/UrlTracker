using Umbraco.Cms.Core.Mapping;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Map
{
    internal class RedirectMap : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<IRedirect, RedirectResponse>(
                (source, context) => new RedirectResponse(),
                (source, target, context) =>
                {
                    target.CreateDate = source.CreateDate;
                    target.Force = source.Force;
                    target.Id = source.Id;
                    target.Key = source.Key;
                    target.Permanent = source.Permanent;
                    target.RetainQuery = source.RetainQuery;
                    target.Source = context.Map<StrategyViewModel>(source.Source)!;
                    target.Target = context.Map<StrategyViewModel>(source.Target)!;
                });

            mapper.Define<RedirectRequest, IRedirect>(
                (source, context) => new RedirectEntity(source.RetainQuery, source.Permanent, source.Force, context.Map<EntityStrategy>(source.Source)!, context.Map<EntityStrategy>(source.Target)!),
                (source, target, context) =>
                {
                    target.Key = source.Key ?? default;
                });

            mapper.Define<EntityStrategy, StrategyViewModel>(
                (source, context) => new StrategyViewModel(),
                (source, target, context) =>
                {
                    target.Strategy = source.Strategy;
                    target.Value = source.Value;
                });

            mapper.Define<StrategyViewModel, EntityStrategy>(
                (source, context) => new EntityStrategy(source.Strategy, source.Value));
        }
    }
}
