using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DigitalPublishingPlatform.Handlers
{
    public class ArticleHandler: ContentHandler
    {
        public ArticleHandler(IRepository<ArticleRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}