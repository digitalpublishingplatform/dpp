using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DigitalPublishingPlatform.Handlers
{
    public class CategoryHandler: ContentHandler
    {
        public CategoryHandler(IRepository<CategoryRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}