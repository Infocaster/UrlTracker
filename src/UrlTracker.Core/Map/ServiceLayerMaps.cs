using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Map
{
    public class ServiceLayerMaps
        : IMapDefinition
    {
        private readonly IStrategyMapCollection _strategyMapCollection;

        public ServiceLayerMaps(IStrategyMapCollection strategyMapCollection)
        {
            _strategyMapCollection = strategyMapCollection;
        }

        [ExcludeFromCodeCoverage]
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<IRedirect, Redirect>(
                (source, context) => new Redirect(),
                Map);

            mapper.Define<Database.Entities.RedirectEntityCollection, Models.RedirectCollection>(
                (source, context) => Models.RedirectCollection.Create(context.MapEnumerable<IRedirect, Redirect>(source), source.Total));

            mapper.Define<Redirect, IRedirect>(
                (source, context) => new RedirectEntity(source.RetainQuery, source.Permanent, source.Force, _strategyMapCollection.Map(source.Source), _strategyMapCollection.Map(source.Target)),
                Map);

            mapper.Define<ClientError, IClientError>(
                (source, context) => new ClientErrorEntity(source.Url, source.Ignored, source.Strategy),
                Map);

            mapper.Define<IClientError, ClientError>(
                (source, context) => new ClientError(source.Url),
                Map);
        }

        private static void Map(IClientError source, ClientError target, MapperContext context)
        {
            target.Id = source.Id;
            target.Ignored = source.Ignored;
            target.Inserted = source.CreateDate;
        }

        private static void Map(ClientError source, IClientError target, MapperContext context)
        {
            target.Id = source.Id;
            target.CreateDate = source.Inserted;
        }

        private void Map(Redirect source, IRedirect target, MapperContext context)
        {
            target.CreateDate = source.Inserted;
            target.Id = source.Id ?? 0;
            target.Key = source.Key ?? default;
        }

        private void Map(IRedirect source, Redirect target, MapperContext context)
        {
            target.Inserted = source.CreateDate;
            target.Force = source.Force;
            target.Id = source.Id == default ? null : source.Id;
            target.Key = source.Key == default ? null : source.Key;
            target.RetainQuery = source.RetainQuery;
            target.Permanent = source.Permanent;
            target.Source = _strategyMapCollection.Map<ISourceStrategy>(source.Source);
            target.Target = _strategyMapCollection.Map<ITargetStrategy>(source.Target);
        }
    }
}
