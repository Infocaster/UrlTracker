namespace UrlTracker.Core.Configuration
{
    // An interface that is similar to the IOptions<T> class in .NET 5. This makes it easier to port later.
    public interface IConfiguration<out T>
    {
        T Value { get; }
    }
}
