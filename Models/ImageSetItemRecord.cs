using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace DigitalPublishingPlatform.Models
{
    public class ImageSetItemRecord 
    {
        public virtual int Id { get; set; }
        public virtual string Url { set; get; }
        public virtual int Position { get; set; }
        public virtual ImageSetRecord ImageSet { set; get; }
    }
}