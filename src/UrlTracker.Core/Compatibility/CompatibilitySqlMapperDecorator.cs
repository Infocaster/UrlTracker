using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Persistence.Mappers;

namespace UrlTracker.Core.Compatibility
{
    public class CompatibilitySqlMapperDecorator : IMapperCollection
    {
        private readonly IMapperCollection _decoratee;

        public CompatibilitySqlMapperDecorator(IMapperCollection decoratee)
        {
            _decoratee = decoratee;
        }

        // maintain our own index for faster lookup
        private readonly ConcurrentDictionary<Type, BaseMapper> _index = new ConcurrentDictionary<Type, BaseMapper>();

        public BaseMapper this[Type type]
        {
            get
            {
                return _index.GetOrAdd(type, t =>
                {
                    // check if any of the mappers are assigned to this type
                    var mapper = this.FirstOrDefault(x => x.GetType()
                        .GetCustomAttributes<CompatibilityMapperForAttribute>(false)
                        .Any(m => m.EntityType == type));

                    if (mapper != null) return mapper;

                    return _decoratee[type];
                });
            }
        }

        public int Count => _decoratee.Count;

        public IEnumerator<BaseMapper> GetEnumerator()
        {
            return _decoratee.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_decoratee).GetEnumerator();
        }
    }
}
