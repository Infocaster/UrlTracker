using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UrlTracker.Core.Abstractions
{
    /// <summary>
    /// When implemented, this type provides an abstraction layer on top of the <see cref="Umbraco.Cms.Core.UmbracoContextReference"/>
    /// </summary>
    public interface IUmbracoContextReferenceAbstraction
        : IDisposable
    {
        /// <summary>
        /// When implemented, this method finds published content with the given id
        /// </summary>
        /// <param name="id">The unique id of the published content</param>
        /// <returns>The <see cref="IPublishedContent"/> with the given id or <see langword="null"/></returns>
        IPublishedContent? GetContentById(int id);

        /// <summary>
        /// When implemented, this method finds media as published content with the given id
        /// </summary>
        /// <param name="id">The unique id of the media</param>
        /// <returns>The <see cref="IPublishedContent"/> with the given id or <see langword="null"/></returns>
        IPublishedContent? GetMediaById(int id);

        /// <summary>
        /// When implemented, this method produces a media url for given published content
        /// </summary>
        /// <param name="content">The content item for which the url should be produced</param>
        /// <param name="mode">The type of url that should be produced</param>
        /// <param name="culture">The culture variant for which the url should be produced</param>
        /// <returns>A <see langword="string" /> that represents the media url</returns>
        string GetMediaUrl(IPublishedContent content, UrlMode mode, string? culture);

        /// <summary>
        /// When implemented, this method produces a url for given published content
        /// </summary>
        /// <param name="content">The content item for which the url should be produced</param>
        /// <param name="mode">The type of url that should be produced</param>
        /// <param name="culture">The culture variant for which the url should be produced</param>
        /// <returns>A <see langword="string" /> that represents the url</returns>
        string GetUrl(IPublishedContent content, UrlMode mode, string? culture);

        /// <summary>
        /// When implemented, this method returns the response code that Umbraco expects to return in the current request
        /// </summary>
        /// <returns>An <see langword="int"/> that represents the response code or <see langword="null"/></returns>
        int? GetResponseCode();

        /// <summary>
        /// When implemented, this method returns all the content items at the root of the content tree in Umbraco
        /// </summary>
        /// <returns>A collection of <see cref="IPublishedContent"/></returns>
        IEnumerable<IPublishedContent> GetContentAtRoot();
    }
}