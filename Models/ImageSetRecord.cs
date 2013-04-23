using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalPublishingPlatform.Models
{
   public class ImageSetRecord: ContentPartRecord
   {
        public ImageSetRecord() {
            ImageList = new List<ImageSetItemRecord>();
        }
        
        public virtual IList<ImageSetItemRecord> ImageList { set; get; }
   }
}
