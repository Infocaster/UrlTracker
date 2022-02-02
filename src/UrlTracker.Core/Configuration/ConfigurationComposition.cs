using LightInject;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace UrlTracker.Core.Configuration
{
    public class ConfigurationComposition<T>
        where T : class
    {
        private readonly IServiceRegistry _container;

        private ConfigurationComposition(IRegister register)
        {
            Register = register;
            _container = Register.Concrete as IServiceRegistry;
        }

        public IRegister Register { get; }

        public IRegister Lazy()
        {
            _container.Decorate<IConfiguration<T>, LazyConfigurationProvider<T>>();
            return Register;
        }

        public static ConfigurationComposition<T> Create<TProvider>(IRegister register)
            where TProvider : IConfiguration<T>
        {
            register.Register<IConfiguration<T>, TProvider>();
            return new ConfigurationComposition<T>(register);
        }
    }
}
