namespace UrlTracker.Core.Abstractions
{
    public interface IUmbracoContextFactoryAbstraction
    {
        IUmbracoContextReferenceAbstraction EnsureUmbracoContext();
    }
}