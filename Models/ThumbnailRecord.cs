using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Models
{
    public class ThumbnailRecord
    {
        public virtual int Id { get; set; }
        public virtual EncodedMediaRecord EncodedMedia { get; set; }
        public virtual string Url { get; set; }
    }
}