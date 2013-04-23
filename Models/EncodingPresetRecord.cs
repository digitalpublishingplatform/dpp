using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Models
{
    public class EncodingPresetRecord
    {        
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual MediaType MediaType { get; set; }
        public virtual MediaDefinition Definition { get; set; }
        public virtual MediaTarget Target { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual int Width { get; set; }
    }
}