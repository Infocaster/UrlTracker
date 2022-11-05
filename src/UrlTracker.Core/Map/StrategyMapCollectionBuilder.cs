using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Map
{
    /// <summary>
    /// A collection builder for mapping complex strategy models to simplified versions and back
    /// </summary>
    public class StrategyMapCollectionBuilder
        : OrderedCollectionBuilderBase<StrategyMapCollectionBuilder, StrategyMapCollection, IStrategyMap>
    {
        /// <inheritdoc />
        protected override StrategyMapCollectionBuilder This => this;
    }

    /// <summary>
    /// When implemented, this type can convert strategies between complex and simplified versions
    /// </summary>
    public interface IStrategyMapCollection
    {
        T Map<T>(EntityStrategy strategy);
        EntityStrategy Map(object strategy);
    }

    /// <summary>
    /// A collection of mappers to map complex strategy models to simplified versions and back
    /// </summary>
    public class StrategyMapCollection : BuilderCollectionBase<IStrategyMap>, IStrategyMapCollection
    {
        /// <inheritdoc />
        public StrategyMapCollection(Func<IEnumerable<IStrategyMap>> items)
            : base(items)
        { }

        public T Map<T>(EntityStrategy strategy)
        {
            var mapper = this.OfType<IStrategyMap<T>>().FirstOrDefault(m => m.CanHandle(strategy));
            if (mapper is null) throw new ArgumentException("Don't know how to map this strategy", nameof(strategy));

            return mapper.Convert(strategy);
        }

        public EntityStrategy Map(object strategy)
        {
            var mapper = this.FirstOrDefault(m => m.CanHandle(strategy));
            if (mapper is null) throw new ArgumentException("Don't know how to map this strategy", nameof(strategy));

            return mapper.Convert(strategy);
        }
    }
}
