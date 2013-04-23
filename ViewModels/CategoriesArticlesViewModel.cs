using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalPublishingPlatform.ViewModels.Category;

namespace DigitalPublishingPlatform.ViewModels
{
    public class CategoriesArticlesViewModel
    {
        public CategoriesArticlesViewModel() 
        {
            CategoryArticleList = new List<CategoryArticleViewModel>();
        }

        public int? MainArticleId { set; get; }
        public IEnumerable<SelectListItem> Articles { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public List<CategoryArticleViewModel> CategoryArticleList { get; set; } 
    }
}