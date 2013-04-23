using System.Collections.Generic;

namespace DigitalPublishingPlatform.ViewModels
{
    public class MainIssueFrontViewModel
    {
        public ArticleViewModel MainArticle { set; get; }
        public IEnumerable<TagArticlesViewModel> TagsAndArticles { set; get; }
    }
}
