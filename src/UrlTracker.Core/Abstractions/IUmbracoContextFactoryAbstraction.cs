namespace UrlTracker.Core.Abstractions
{
    /// <summary>
    /// When implemented, this type provides an abstraction layer on top of the <see cref="Umbraco.Cms.Core.Web.IUmbracoContextFactory"/>
    /// </summary>
    public interface IUmbracoContextFactoryAbstraction
    {
        /// <summary>
        /// When implemented, this method creates an <see cref="IUmbracoContextReferenceAbstraction"/>
        /// </summary>
        /// <returns>A new instance of <see cref="IUmbracoContextReferenceAbstraction"/></returns>
        /// <remarks>
        /// <para>Return value should either be wrapped in a <see langword="using"/> statement or be disposed explicitly after use</para>
        /// </remarks>
        IUmbracoContextReferenceAbstraction EnsureUmbracoContext();
    }
}