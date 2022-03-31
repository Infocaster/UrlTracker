using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestPublishedContent : IPublishedContent
    {
        public TestPublishedContent(bool addDefaultCulture = true)
        {
            if (addDefaultCulture)
            {
                Cultures = new Dictionary<string, PublishedCultureInfo>
                {
                    {"nl-nl", new PublishedCultureInfo("nl-nl", "Dutch", "test", new DateTime(1970, 1, 1)) }
                };
            }
        }

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? UrlSegment { get; set; }

        public int SortOrder { get; set; }

        public int Level { get; set; }

        public string? Path { get; set; }

        public int? TemplateId { get; set; }

        public int CreatorId { get; set; }

        public string? CreatorName { get; set; }

        public DateTime CreateDate { get; set; }

        public int WriterId { get; set; }

        public string? WriterName { get; set; }

        public DateTime UpdateDate { get; set; }

        public string? Url { get; set; }

        public IReadOnlyDictionary<string, PublishedCultureInfo>? Cultures { get; set; }

        public PublishedItemType ItemType { get; set; }

        public IPublishedContent? Parent { get; set; }

        public IEnumerable<IPublishedContent>? Children { get; set; }

        public IEnumerable<IPublishedContent>? ChildrenForAllCultures { get; set; }

        public IPublishedContentType? ContentType { get; set; }

        public Guid Key { get; set; }

        public IEnumerable<IPublishedProperty>? Properties { get; set; }

        public IPublishedProperty GetProperty(string alias)
        {
            throw new NotImplementedException();
        }

        public bool IsDraft(string? culture = null)
        {
            throw new NotImplementedException();
        }

        public bool IsPublished(string? culture = null)
        {
            throw new NotImplementedException();
        }
    }
}
