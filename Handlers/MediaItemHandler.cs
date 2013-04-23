using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Handlers
{
    public class MediaItemHandler: ContentHandler
    {
        public MediaItemHandler(IRepository<MediaItemRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}