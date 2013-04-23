using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class ArticleItemListViewModel
    {
        public  ArticleItemListViewModel() 
        {
            ArticleItems = new List<ArticleItemViewModel>();
        }
        public IEnumerable<ArticleItemViewModel> ArticleItems { set; get; }
        public int IssueId { set; get; }
        public dynamic Pager { get; set; }
        public string Find { get; set; }
    }
}