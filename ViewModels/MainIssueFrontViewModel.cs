using System.Collections.Generic;

namespace DigitalPublishingPlatform.ViewModels
{
    public class MainIssueFrontViewModel
    {
        public ArticleViewModel MainArticle { set; get; }
        public IEnumerable<CategoryArticlesViewModel> CategoriesAndArticles { set; get; }
    }
}
