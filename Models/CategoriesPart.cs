using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace DigitalPublishingPlatform.Models
{
    public class CategoriesPart : ContentPart<CategoryRecord>
    {
        public IEnumerable<ArticleRecord> ArticleList
        {
            get
            {
                return Record.ArticleCategoryRecords.Select(mi => mi.ArticleRecord);
            }
        }

        public ArticlePart ArticlePart
        {
            get { return this.As<ICommonPart>().Container.As<ArticlePart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }
    }
}