using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace DigitalPublishingPlatform.Models
{
    public class ImageSetPart : ContentPart<ImageSetRecord>
    {        
        
        public ArticlePart ArticlePart
        {
            get { return this.As<ICommonPart>().Container.As<ArticlePart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        
    }
}