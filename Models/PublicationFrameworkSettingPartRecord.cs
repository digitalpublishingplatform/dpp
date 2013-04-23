using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Models
{
    public class PublicationFrameworkSettingPartRecord: ContentPartRecord
    {
        public virtual string MediaServiceAccount { get; set; }
        public virtual string MediaServiceKey { get; set; }
        public virtual string BlobStorageAccount { get; set; }
        public virtual string BlobStorageKey { get; set; }
    }
}