using UrlTracker.Core.Configuration;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestConfiguration<T> : IConfiguration<T>
    {
        public T Value { get; set; }
    }
}
