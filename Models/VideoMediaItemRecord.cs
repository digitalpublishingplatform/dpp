using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace DigitalPublishingPlatform.Models
{
    public class VideoMediaItemRecord 
    {
        public virtual int Id { get; set; }
        public virtual MediaItemRecord MediaItemRecord { get; set; }
        public virtual VideoRecord VideoRecord { get; set; }
        public virtual int Position { get; set; }
        
    }
}