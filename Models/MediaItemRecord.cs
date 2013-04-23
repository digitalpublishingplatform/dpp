using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace DigitalPublishingPlatform.Models
{
    public class MediaItemRecord : ContentPartRecord
    {
        public MediaItemRecord() {
            EncodedMediaList = new List<EncodedMediaRecord>();
        }
        public virtual string Url { get; set; }     
        public virtual MediaType Type { get; set; }
        public virtual string MimeType { get; set; }
        public virtual string Filename { get; set; }                
        public virtual string AssetId { get; set; }
        public virtual long Size { get; set; }
        public virtual IList<EncodedMediaRecord> EncodedMediaList { get; set; }
        public virtual string DefaultThumbnailUrl { get; set; }
        public virtual string FileToken { get; set; }
    }
}