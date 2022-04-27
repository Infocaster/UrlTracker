using Umbraco.Core.Mapping;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestMapDefinition<TFrom, TTo> : IMapDefinition
    {
        public TTo To { get; set; }

        public void DefineMaps(UmbracoMapper mapper)
        {
            mapper.Define<TFrom, TTo>((source, context) => To);
        }
    }
}
