using System.Collections.Generic;
using DigitalPublishingPlatform.ViewModels.Category;

namespace DigitalPublishingPlatform.ViewModels
{
    public class IssueFrontViewModel
    {
        public int SelectedArticleId { get; set; }
        public IEnumerable<ArticleItemViewModel> Articles { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
