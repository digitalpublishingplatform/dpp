using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DigitalPublishingPlatform.Handlers
{
    public class VideoHandler: ContentHandler
    {   
        public VideoHandler(IRepository<VideoRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}