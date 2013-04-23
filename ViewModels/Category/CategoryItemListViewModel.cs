using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalPublishingPlatform.ViewModels.Category
{
    public class CategoryItemListViewModel
    {
        public CategoryItemListViewModel() 
        {
            Categories = new List<CategoryViewModel>();
        }
        public IEnumerable<CategoryViewModel> Categories { set; get; }        
        public dynamic Pager { get; set; }
    }
}
