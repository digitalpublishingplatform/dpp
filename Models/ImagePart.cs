using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Aspects;

namespace DigitalPublishingPlatform.Models
{
    public class ImagePart : ContentPart<ImageRecord>
    {
        public string Url {
            set { Record.Url = value; }
            get { return Record.Url; }
        }
    }
}