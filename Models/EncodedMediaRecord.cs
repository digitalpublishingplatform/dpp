using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace DigitalPublishingPlatform.Models
{
    public class EncodedMediaRecord
    {
        public virtual int Id { set; get; }
        public virtual string JobErrorMessage { set; get; }
        public virtual string JobId { set; get; }
        public virtual string AssetId { set; get; } 
        public virtual string Url { get; set; }       
        public virtual string Status { set; get; }
        [StringLengthMax]
        public virtual string Metadata { set; get; }
        public virtual DateTime? CreatedUtc { set; get; }
        public virtual DateTime? ModifiedUtc { get; set; }
        public virtual string Owner { get; set; }
        [CascadeAllDeleteOrphan]
        public virtual EncodingPresetRecord EncodingPreset { set; get; }
        [CascadeAllDeleteOrphan]
        public virtual MediaItemRecord MediaItem { set; get; }
        public virtual int Width { set; get; }
        public virtual int Height { set; get; }
        public virtual decimal Framerate { set; get; }
        public virtual decimal AspectRatio { get; set; }
    }
}