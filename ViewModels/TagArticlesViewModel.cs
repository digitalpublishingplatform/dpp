using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalPublishingPlatform.ViewModels
{
    public class TagArticlesViewModel
    {
        public string Tag { set; get; }
        public IEnumerable<ArticleViewModel> Articles { set; get; }
    }
}
