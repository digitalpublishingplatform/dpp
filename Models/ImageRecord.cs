using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace DigitalPublishingPlatform.Models
{
    public class ImageRecord : ContentPartRecord
    {
        public virtual string Url { get; set; }        
    }
}