using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DigitalPublishingPlatform.Handlers
{
    public class PublicationFrameworkSettingPartHandler : ContentHandler
    {
        public PublicationFrameworkSettingPartHandler(IRepository<PublicationFrameworkSettingPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<PublicationFrameworkSettingPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}