namespace UrlTracker.Core.Intercepting.Models
{
    public interface IIntercept
    {
        object Info { get; }
    }

    public interface IIntercept<out TInfo>
        : IIntercept
    {
        new TInfo Info { get; }
    }

    public interface ICachableIntercept
        : IIntercept
    { }

    public interface ICachableIntercept<out TInfo>
        : IIntercept<TInfo>, ICachableIntercept
    { }
}
