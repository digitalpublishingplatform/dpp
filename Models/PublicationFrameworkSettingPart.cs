using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Models
{
    public class PublicationFrameworkSettingPart:ContentPart<PublicationFrameworkSettingPartRecord>
    {
        public string MediaServiceAccount
        {
            set { Record.MediaServiceAccount = value; }
            get { return Record.MediaServiceAccount; }
        }
        
        public string MediaServiceKey
        {
            set { Record.MediaServiceKey= value; }
            get { return Record.MediaServiceKey; }
        }

        public string BlobStorageAccount
        {
            set { Record.BlobStorageAccount = value; }
            get { return Record.BlobStorageAccount; }
        }

        public string BlobStorageKey
        {
            set { Record.BlobStorageKey = value; }
            get { return Record.BlobStorageKey; }
        }
    }
}