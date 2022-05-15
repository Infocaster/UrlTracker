using System;
using System.Collections.Generic;
using Moq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UrlTracker.Resources.Testing.Objects
{
    public static class TestPublishedContent
    {
        public static IPublishedContent Create(int id, PublishedItemType itemType = PublishedItemType.Content, bool addDefaultCulture = true)
        {
            var contentMock = new Mock<IPublishedContent>();
            contentMock.Setup(x => x.Id).Returns(id);
            contentMock.Setup(x => x.ItemType).Returns(itemType);
            if (addDefaultCulture)
            {
                contentMock.Setup(x => x.Cultures).Returns(new Dictionary<string, PublishedCultureInfo>
                {
                    {"nl-nl", new PublishedCultureInfo("nl-nl", "Dutch", "test", new DateTime(1970, 1, 1)) }
                });
            }

            return contentMock.Object;
        }
    }
}
