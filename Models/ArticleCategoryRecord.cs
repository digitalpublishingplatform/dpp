using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Models
{
    public class ArticleCategoryRecord
    {
        public virtual int Id { get; set; }
        public virtual ArticleRecord ArticleRecord { get; set; }
        public virtual CategoryRecord CategoryRecord { get; set; }
        public virtual int? CategoryDisplayOrder { get; set; }
    }
}