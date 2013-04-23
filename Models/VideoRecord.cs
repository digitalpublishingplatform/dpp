using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace DigitalPublishingPlatform.Models
{
    public class VideoRecord : ContentPartRecord
    {
        public VideoRecord()
        {
            MediaItemList = new List<VideoMediaItemRecord>();
        }
        public virtual IList<VideoMediaItemRecord> MediaItemList { get; set; }
    }
}