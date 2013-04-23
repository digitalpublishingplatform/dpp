using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalPublishingPlatform.Models
{
    public class ArticleRecord: ContentPartRecord
    {
        public ArticleRecord() {
            ArticleCategoryRecords = new List<ArticleCategoryRecord>();
        }
        public virtual string Author { get; set; }
        public virtual bool MainArticle { get; set; }

        public virtual IList<ArticleCategoryRecord> ArticleCategoryRecords { get; set; }
    }
}
