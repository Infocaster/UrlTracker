using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace UrlTracker.Resources.Website.Composing
{
    public class AttributeRoutingComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<AttributeRoutingComponent>();
        }

        public class AttributeRoutingComponent : IComponent
        {
            public void Initialize()
            {
                GlobalConfiguration.Configuration.MapHttpAttributeRoutes();
            }

            public void Terminate()
            { }
        }
    }
}