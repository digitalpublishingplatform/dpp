using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Models
{
    public class CategoryRecord: ContentPartRecord
    {
        public CategoryRecord() {
            ArticleCategoryRecords = new List<ArticleCategoryRecord>();
        }
        public virtual string Name { get; set; }
        public virtual int Position { get; set; }
        public virtual IList<ArticleCategoryRecord> ArticleCategoryRecords  { get; set; }
        public virtual int PublicationId { get; set; }
       
    }
}