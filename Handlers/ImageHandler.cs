using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DigitalPublishingPlatform.Handlers
{
    public class ImageHandler : ContentHandler
    {
        public ImageHandler(IRepository<ImageRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}