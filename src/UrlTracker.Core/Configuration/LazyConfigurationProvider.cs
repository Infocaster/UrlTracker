using System;

namespace UrlTracker.Core.Configuration
{
    public class LazyConfigurationProvider<T>
        : IConfiguration<T>
    {
        private readonly Lazy<T> _lazyValue;
        private readonly IConfiguration<T> _decoratee;

        public LazyConfigurationProvider(IConfiguration<T> decoratee)
        {
            _decoratee = decoratee;
            _lazyValue = new Lazy<T>(Create);
        }

        protected virtual T Create()
        {
            return _decoratee.Value;
        }

        public T Value => _lazyValue.Value;
    }
}
